using ElsaMina.Commands.Badges;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Badges;

public class DeleteBadgeTest
{
    private IContext _context;
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private DbSet<Badge> _badgesDbSet;
    private DeleteBadge _command;

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

        _command = new DeleteBadge(_dbContextFactory);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        // Assert
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithDoesntExistMessage_WhenNonExistingBadge()
    {
        // Arrange
        _context.Target.Returns("NonExistingBadge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_delete_doesnt_exist", "nonexistingbadge");
        _dbContext.Badges.DidNotReceive().Remove(Arg.Any<Badge>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDeleteBadgeAndReplyWithSuccessMessage_WhenExistingBadge()
    {
        // Arrange
        var existingBadge = new Badge { Id = "existingbadge", RoomId = "room1" };
        _context.Target.Returns("ExistingBadge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingBadge);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _dbContext.Badges.Received(1).Remove(existingBadge);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("badge_delete_success", "existingbadge");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFailureMessage_WhenFindAsyncThrowsException()
    {
        // Arrange
        _context.Target.Returns("badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Database error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_delete_failure", "Database error");
        _context.DidNotReceive().ReplyLocalizedMessage("badge_delete_success", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFailureMessage_WhenSaveChangesThrowsException()
    {
        // Arrange
        var existingBadge = new Badge { Id = "badge", RoomId = "room1" };
        _context.Target.Returns("badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingBadge);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception("Save failed"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_delete_failure", "Save failed");
        _context.DidNotReceive().ReplyLocalizedMessage("badge_delete_success", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizeBadgeId_ToLowerAlphaNum()
    {
        // Arrange
        _context.Target.Returns("Test-Badge_123!");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => 
                arr.Length == 2 && 
                arr[0].ToString() == "testbadge123"),
            Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("badge_delete_doesnt_exist", "testbadge123");
    }

    [Test]
    public async Task Test_RunAsync_ShouldFindBadge_UsingBadgeIdAndRoomId()
    {
        // Arrange
        _context.Target.Returns("badge");
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
    public async Task Test_RunAsync_ShouldPassCancellationToken_ToAllDbOperations()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var existingBadge = new Badge { Id = "badge", RoomId = "room1" };
        _context.Target.Returns("badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingBadge);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context, cancellationToken);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(cancellationToken);
        await _badgesDbSet.Received(1).FindAsync(Arg.Any<object[]>(), cancellationToken);
        await _dbContext.Received(1).SaveChangesAsync(cancellationToken);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_AfterSuccessfulExecution()
    {
        // Arrange
        var existingBadge = new Badge { Id = "badge", RoomId = "room1" };
        _context.Target.Returns("badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingBadge);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_WhenBadgeNotFound()
    {
        // Arrange
        _context.Target.Returns("badge");
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
        _context.Target.Returns("badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotCallSaveChanges_WhenBadgeNotFound()
    {
        // Arrange
        _context.Target.Returns("nonexistent");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldRemoveBadge_BeforeSavingChanges()
    {
        // Arrange
        var existingBadge = new Badge { Id = "badge", RoomId = "room1" };
        _context.Target.Returns("badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingBadge);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var callOrder = new List<string>();
        _dbContext.Badges.When(x => x.Remove(Arg.Any<Badge>()))
            .Do(_ => callOrder.Add("remove"));
        _dbContext.When(x => x.SaveChangesAsync(Arg.Any<CancellationToken>()))
            .Do(_ => callOrder.Add("save"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(callOrder, Is.EqualTo(new[] { "remove", "save" }));
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleEmptyTarget()
    {
        // Arrange
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_delete_doesnt_exist", string.Empty);
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleNullTarget()
    {
        // Arrange
        _context.Target.Returns((string)null);
        _context.RoomId.Returns("room1");

        // Act & Assert - this will throw a NullReferenceException in the actual code
        // This test documents current behavior; you may want to add null checking in the actual implementation
        Assert.ThrowsAsync<NullReferenceException>(async () => await _command.RunAsync(_context));
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseCorrectRoomId_FromContext()
    {
        // Arrange
        _context.Target.Returns("badge");
        _context.RoomId.Returns("myroom");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgesDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => arr[1].ToString() == "myroom"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldOnlyRemoveSpecificBadge()
    {
        // Arrange
        var badgeToDelete = new Badge { Id = "badge1", RoomId = "room1" };
        _context.Target.Returns("badge1");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(badgeToDelete);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _dbContext.Badges.Received(1).Remove(Arg.Is<Badge>(b => b.Id == "badge1" && b.RoomId == "room1"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldCatchAllExceptions()
    {
        // Arrange
        _context.Target.Returns("badge");
        _context.RoomId.Returns("room1");
        _badgesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Unexpected error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_delete_failure", "Unexpected error");
    }
}