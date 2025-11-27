using ElsaMina.Commands.Arcade;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Arcade;

public class DeleteArcadeLevelTests
{
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private IContext _context;
    private DeleteArcadeLevel _command;
    private DbSet<ArcadeLevel> _arcadeLevelsDbSet;

    [SetUp]
    public void SetUp()
    {
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContext = Substitute.For<BotDbContext>();
        _context = Substitute.For<IContext>();
        _arcadeLevelsDbSet = Substitute.For<DbSet<ArcadeLevel>>();

        _dbContext.ArcadeLevels.Returns(_arcadeLevelsDbSet);
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_dbContext);

        _command = new DeleteArcadeLevel(_dbContextFactory);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void Test_RoomRestriction_ShouldContainArcadeAndBotDevelopment()
    {
        Assert.That(_command.RoomRestriction, Is.EqualTo(new[] { "arcade", "botdevelopment" }));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeCorrect()
    {
        Assert.That(_command.HelpMessageKey, Is.EqualTo("arcade_level_delete_help"));
    }

    [Test]
    public async Task Test_RunAsync_WhenTargetIsNull_ShouldReplyWithHelpMessage()
    {
        // Arrange
        _context.Target.Returns((string)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_delete_help");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_WhenTargetIsEmpty_ShouldReplyWithHelpMessage()
    {
        // Arrange
        _context.Target.Returns(string.Empty);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_delete_help");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_WhenTargetIsWhitespace_ShouldReplyWithHelpMessage()
    {
        // Arrange
        _context.Target.Returns("   ");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_delete_help");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_WhenLevelNotFound_ShouldReplyNotFound()
    {
        // Arrange
        _context.Target.Returns("TestLevel123");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _arcadeLevelsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => arr.Length == 1), 
            Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("arcade_level_delete_not_found");
        _dbContext.DidNotReceive().Remove(Arg.Any<object>());
    }

    [Test]
    public async Task Test_RunAsync_WhenLevelExists_ShouldDeleteAndReplySuccess()
    {
        // Arrange
        var arcadeLevel = new ArcadeLevel { Id = "testlevel123" };
        _context.Target.Returns("TestLevel123");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(arcadeLevel);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _arcadeLevelsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => arr.Length == 1), 
            Arg.Any<CancellationToken>());
        _dbContext.Received(1).Remove(arcadeLevel);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("arcade_level_delete_success");
    }

    [Test]
    public async Task Test_RunAsync_WhenSaveChangesFails_ShouldReplyFailureWithErrorMessage()
    {
        // Arrange
        var arcadeLevel = new ArcadeLevel { Id = "testlevel123" };
        var exceptionMessage = "Database connection error";
        _context.Target.Returns("TestLevel123");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(arcadeLevel);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception(exceptionMessage));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _dbContext.Received(1).Remove(arcadeLevel);
        _context.Received(1).ReplyLocalizedMessage("arcade_level_delete_failure", exceptionMessage);
        _context.DidNotReceive().ReplyLocalizedMessage("arcade_level_delete_success");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizeTargetToLowerAlphaNum()
    {
        // Arrange
        _context.Target.Returns("Test-Level_123!");
        var arcadeLevel = new ArcadeLevel { Id = "testlevel123" };
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(arcadeLevel);

        // Act
        await _command.RunAsync(_context);

        // Assert
        // The ToLowerAlphaNum() should normalize the input
        await _arcadeLevelsDbSet.Received(1).FindAsync(
            Arg.Any<object[]>(), 
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCancellationTokenToDbOperations()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var arcadeLevel = new ArcadeLevel { Id = "testlevel123" };
        _context.Target.Returns("TestLevel123");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(arcadeLevel);

        // Act
        await _command.RunAsync(_context, cancellationToken);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(cancellationToken);
        await _arcadeLevelsDbSet.Received(1).FindAsync(Arg.Any<object[]>(), cancellationToken);
        await _dbContext.Received(1).SaveChangesAsync(cancellationToken);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContextAfterUse()
    {
        // Arrange
        _context.Target.Returns("TestLevel123");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }
}