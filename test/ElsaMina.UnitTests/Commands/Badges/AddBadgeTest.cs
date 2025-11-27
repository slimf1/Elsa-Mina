using ElsaMina.Commands.Badges;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Badges;

public class AddBadgeTest
{
    private IContext _context;
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private DbSet<Badge> _badgesDbSet;
    private AddBadge _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContext = Substitute.For<BotDbContext>();
        _badgesDbSet = Substitute.For<DbSet<Badge>>();

        _dbContext.Badges.Returns(_badgesDbSet);
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_dbContext);

        _command = new AddBadge(_dbContextFactory);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        // Assert
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenArgumentsAreMissing()
    {
        // Arrange
        _context.Target.Returns("invalid");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_help_message");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenNoCommaInTarget()
    {
        // Arrange
        _context.Target.Returns("badgename");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_help_message");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenTooManyArguments()
    {
        // Arrange
        _context.Target.Returns("badge, image, extra");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_help_message");
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
        _context.Received(1).ReplyLocalizedMessage("badge_help_message");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithAlreadyExistMessage_WhenBadgeExists()
    {
        // Arrange
        _context.Target.Returns("ExistingBadge, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        var existingBadge = new Badge { Id = "existingbadge", RoomId = "room1" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingBadge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_add_already_exist", "ExistingBadge");
        await _badgesDbSet.DidNotReceive().AddAsync(Arg.Any<Badge>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldAddBadgeAndReplyWithSuccessMessage_WhenArgumentsAreValid()
    {
        // Arrange
        _context.Target.Returns("NewBadge, http://image.url");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).AddAsync(
            Arg.Is<Badge>(b =>
                b.Name == "NewBadge" &&
                b.Image == "http://image.url" &&
                b.Id == "newbadge" &&
                b.IsTrophy == false &&
                b.RoomId == "room1"),
            Arg.Any<CancellationToken>());
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("badge_add_success_message");
    }

    [Test]
    [TestCase("add-trophy")]
    [TestCase("newtrophy")]
    [TestCase("new-trophy")]
    public async Task Test_RunAsync_ShouldSetIsTrophyToTrue_WhenCommandIsTrophyVariant(string command)
    {
        // Arrange
        _context.Target.Returns("Trophy, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns(command);
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).AddAsync(
            Arg.Is<Badge>(b => b.IsTrophy == true),
            Arg.Any<CancellationToken>());
    }

    [Test]
    [TestCase("add-badge")]
    [TestCase("addbadge")]
    [TestCase("new-badge")]
    [TestCase("newbadge")]
    public async Task Test_RunAsync_ShouldSetIsTrophyToFalse_WhenCommandIsBadgeVariant(string command)
    {
        // Arrange
        _context.Target.Returns("Badge, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns(command);
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).AddAsync(
            Arg.Is<Badge>(b => b.IsTrophy == false),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFailureMessage_WhenAddAsyncThrowsException()
    {
        // Arrange
        _context.Target.Returns("NewBadge, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _badgesDbSet.AddAsync(Arg.Any<Badge>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Database error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_add_failure_message", "Database error");
        _context.DidNotReceive().ReplyLocalizedMessage("badge_add_success_message");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFailureMessage_WhenSaveChangesThrowsException()
    {
        // Arrange
        _context.Target.Returns("NewBadge, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception("Save failed"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_add_failure_message", "Save failed");
        _context.DidNotReceive().ReplyLocalizedMessage("badge_add_success_message");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizeBadgeName_ToLowerAlphaNum()
    {
        // Arrange
        _context.Target.Returns("Test-Badge_123!, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).AddAsync(
            Arg.Is<Badge>(b => b.Id == "testbadge123"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimWhitespace_FromNameAndImage()
    {
        // Arrange
        _context.Target.Returns("  Badge Name  ,  http://image.url  ");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).AddAsync(
            Arg.Is<Badge>(b =>
                b.Name == "Badge Name" &&
                b.Image == "http://image.url"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCancellationToken_ToAllDbOperations()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _context.Target.Returns("Badge, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context, cancellationToken);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(cancellationToken);
        await _badgesDbSet.Received(1).FindAsync(Arg.Any<object[]>(), cancellationToken);
        await _badgesDbSet.Received(1).AddAsync(Arg.Any<Badge>(), cancellationToken);
        await _dbContext.Received(1).SaveChangesAsync(cancellationToken);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_AfterExecution()
    {
        // Arrange
        _context.Target.Returns("Badge, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_EvenWhenExceptionOccurs()
    {
        // Arrange
        _context.Target.Returns("Badge, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception("Error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldCheckExistingBadge_UsingBadgeIdAndRoomId()
    {
        // Arrange
        _context.Target.Returns("TestBadge, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => 
                arr.Length == 2 && 
                arr[0].ToString() == "testbadge" && 
                arr[1].ToString() == "room1"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetCorrectRoomId_InBadge()
    {
        // Arrange
        _context.Target.Returns("Badge, image");
        _context.RoomId.Returns("specificroom");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).AddAsync(
            Arg.Is<Badge>(b => b.RoomId == "specificroom"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldPreserveOriginalName_WhileNormalizingId()
    {
        // Arrange
        _context.Target.Returns("Cool Badge!, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).AddAsync(
            Arg.Is<Badge>(b => 
                b.Name == "Cool Badge!" && 
                b.Id == "coolbadge"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotCallSaveChanges_WhenBadgeAlreadyExists()
    {
        // Arrange
        _context.Target.Returns("ExistingBadge, image");
        _context.RoomId.Returns("room1");
        _context.Command.Returns("add-badge");
        var existingBadge = new Badge { Id = "existingbadge", RoomId = "room1" };
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingBadge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotCallSaveChanges_WhenValidationFails()
    {
        // Arrange
        _context.Target.Returns("invalid");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}