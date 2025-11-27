using ElsaMina.Commands.Badges;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace ElsaMina.UnitTests.Commands.Badges;

[TestFixture]
public class GiveBadgeTest
{
    private IContext _context;
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private DbSet<Badge> _badgesDbSet;
    private IRoomUserDataService _roomUserDataService;
    private GiveBadge _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContext = Substitute.For<BotDbContext>();
        _badgesDbSet = Substitute.For<DbSet<Badge>>();
        _roomUserDataService = Substitute.For<IRoomUserDataService>();

        _dbContext.Badges.Returns(_badgesDbSet);
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_dbContext);

        _command = new GiveBadge(_roomUserDataService, _dbContextFactory);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        // Assert
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldReturnCorrectKey()
    {
        // Assert
        Assert.That(_command.HelpMessageKey, Is.EqualTo("badge_give_help_message"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenInvalidArguments()
    {
        // Arrange
        _context.Target.Returns("invalid");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_give_help_message");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenNoCommaInTarget()
    {
        // Arrange
        _context.Target.Returns("justoneargument");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_give_help_message");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenTooManyArguments()
    {
        // Arrange
        _context.Target.Returns("user,badge,extra");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_give_help_message");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenTargetIsEmpty()
    {
        // Arrange
        _context.Target.Returns(string.Empty);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_give_help_message");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithCouldNotFindBadgeMessage_WhenBadgeNotFound()
    {
        // Arrange
        _context.Target.Returns("UserId,NonExistingBadge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_give_could_not_find_badge", "nonexistingbadge");
        await _roomUserDataService.DidNotReceive().GiveBadgeToUserAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldGiveBadgeToUserAndReplyWithSuccessMessage_WhenValidArguments()
    {
        // Arrange
        var badgeId = "existingbadge";
        var badgeName = "Existing Badge";
        _context.Target.Returns("UserId,ExistingBadge");
        _context.RoomId.Returns("room1");
        var existingBadge = new Badge { Id = badgeId, Name = badgeName, RoomId = "room1" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingBadge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _roomUserDataService.Received(1).GiveBadgeToUserAsync("room1", "userid", badgeId);
        _context.Received(1).ReplyLocalizedMessage("badge_give_success", "userid", badgeName);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithErrorMessage_WhenGiveBadgeThrowsException()
    {
        // Arrange
        _context.Target.Returns("UserId,ExistingBadge");
        _context.RoomId.Returns("room1");
        var existingBadge = new Badge { Id = "existingbadge", Name = "Badge", RoomId = "room1" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingBadge);
        _roomUserDataService.GiveBadgeToUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Throws(new Exception("Service error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_give_error", "Service error");
        _context.DidNotReceive().ReplyLocalizedMessage("badge_give_success", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithCouldNotFindBadge_WhenFindAsyncThrowsException()
    {
        // Arrange
        _context.Target.Returns("UserId,Badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Database error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_give_could_not_find_badge", "badge");
        await _roomUserDataService.DidNotReceive().GiveBadgeToUserAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizeUserIdAndBadgeId_ToLowerAlphaNum()
    {
        // Arrange
        _context.Target.Returns("Test-User_123!,Cool-Badge_456!");
        _context.RoomId.Returns("room1");
        var badge = new Badge { Id = "coolbadge456", Name = "Cool Badge", RoomId = "room1" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(badge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => arr[0].ToString() == "coolbadge456"),
            Arg.Any<CancellationToken>());
        await _roomUserDataService.Received(1).GiveBadgeToUserAsync("room1", "testuser123", "coolbadge456");
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimWhitespace_FromUserIdAndBadgeId()
    {
        // Arrange
        _context.Target.Returns("  UserId  ,  BadgeId  ");
        _context.RoomId.Returns("room1");
        var badge = new Badge { Id = "badgeid", Name = "Badge", RoomId = "room1" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(badge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _roomUserDataService.Received(1).GiveBadgeToUserAsync("room1", "userid", "badgeid");
    }

    [Test]
    public async Task Test_RunAsync_ShouldFindBadge_UsingBadgeIdAndRoomId()
    {
        // Arrange
        _context.Target.Returns("user,badge");
        _context.RoomId.Returns("specificroom");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => 
                arr.Length == 2 && 
                arr[0].ToString() == "badge" && 
                arr[1].ToString() == "specificroom"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCancellationToken_ToDbOperations()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _context.Target.Returns("user,badge");
        _context.RoomId.Returns("room1");
        var badge = new Badge { Id = "badge", Name = "Badge", RoomId = "room1" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(badge);

        // Act
        await _command.RunAsync(_context, cancellationToken);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(cancellationToken);
        await _badgesDbSet.Received(1).FindAsync(Arg.Any<object[]>(), cancellationToken);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_AfterSuccessfulExecution()
    {
        // Arrange
        _context.Target.Returns("user,badge");
        _context.RoomId.Returns("room1");
        var badge = new Badge { Id = "badge", Name = "Badge", RoomId = "room1" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(badge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_WhenBadgeNotFound()
    {
        // Arrange
        _context.Target.Returns("user,badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_EvenWhenExceptionOccurs()
    {
        // Arrange
        _context.Target.Returns("user,badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseCorrectRoomId_FromContext()
    {
        // Arrange
        _context.Target.Returns("user,badge");
        _context.RoomId.Returns("myroom");
        var badge = new Badge { Id = "badge", Name = "Badge", RoomId = "myroom" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(badge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _roomUserDataService.Received(1).GiveBadgeToUserAsync("myroom", Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassBadgeName_InSuccessMessage()
    {
        // Arrange
        _context.Target.Returns("user,badge");
        _context.RoomId.Returns("room1");
        var badge = new Badge { Id = "badge", Name = "Awesome Badge", RoomId = "room1" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(badge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_give_success", "user", "Awesome Badge");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotCallGiveBadge_WhenBadgeNotFound()
    {
        // Arrange
        _context.Target.Returns("user,nonexistent");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _roomUserDataService.DidNotReceive().GiveBadgeToUserAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotCallGiveBadge_WhenValidationFails()
    {
        // Arrange
        _context.Target.Returns("invalid");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _roomUserDataService.DidNotReceive().GiveBadgeToUserAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleEmptyParts_AfterSplit()
    {
        // Arrange
        _context.Target.Returns(",");

        // Act
        await _command.RunAsync(_context);

        // Assert
        // Split will produce 2 empty strings, which is valid count but will fail normalization
        // The command should handle this gracefully
        await _dbContextFactory.Received(1).CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldCallGiveBadgeService_WithCorrectParameters()
    {
        // Arrange
        _context.Target.Returns("testuser,testbadge");
        _context.RoomId.Returns("testroom");
        var badge = new Badge { Id = "testbadge", Name = "Test Badge", RoomId = "testroom" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(badge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _roomUserDataService.Received(1).GiveBadgeToUserAsync(
            "testroom", 
            "testuser", 
            "testbadge");
    }
}