using ElsaMina.Commands.Arcade;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Arcade;

public class SetArcadeLevelTests
{
    private SetArcadeLevel _command;
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private IContext _context;
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

        _command = new SetArcadeLevel(_dbContextFactory);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        // Assert
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void Test_RoomRestriction_ShouldContainArcadeAndBotDevelopment()
    {
        // Assert
        Assert.That(_command.RoomRestriction, Is.EqualTo(new[] { "arcade", "botdevelopment" }));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldReturnCorrectKey()
    {
        // Assert
        Assert.That(_command.HelpMessageKey, Is.EqualTo("arcade_level_help"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelpMessage_WhenInputIsInvalid()
    {
        // Arrange
        _context.Target.Returns("invalid_input");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_help");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelpMessage_WhenInputHasNoComma()
    {
        // Arrange
        _context.Target.Returns("useronly");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_help");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelpMessage_WhenLevelIsNotANumber()
    {
        // Arrange
        _context.Target.Returns("user,notanumber");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_help");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelpMessage_WhenTargetIsNull()
    {
        // Arrange
        _context.Target.Returns((string)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_help");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    [TestCase("0")]
    [TestCase("-1")]
    [TestCase("-10")]
    public async Task Test_RunAsync_ShouldReplyError_WhenLevelIsBelowMinimum(string invalidValue)
    {
        // Arrange
        _context.Target.Returns($"user,{invalidValue}");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_invalid_value");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    [TestCase("51")]
    [TestCase("100")]
    [TestCase("999")]
    public async Task Test_RunAsync_ShouldReplyError_WhenLevelIsAboveMaximum(string invalidValue)
    {
        // Arrange
        _context.Target.Returns($"user,{invalidValue}");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_invalid_value");
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    [TestCase("1")]
    [TestCase("25")]
    [TestCase("50")]
    public async Task Test_RunAsync_ShouldAcceptValidLevels(string validValue)
    {
        // Arrange
        _context.Target.Returns($"user,{validValue}");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _arcadeLevelsDbSet.Received(1).AddAsync(
            Arg.Is<ArcadeLevel>(level => level.Level == int.Parse(validValue)),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldAddLevel_WhenUserDoesNotExist()
    {
        // Arrange
        _context.Target.Returns("TestUser,3");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _arcadeLevelsDbSet.Received(1).AddAsync(
            Arg.Is<ArcadeLevel>(level => level.Id == "testuser" && level.Level == 3),
            Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("arcade_level_add", "testuser", 3);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldUpdateLevel_WhenUserExists()
    {
        // Arrange
        _context.Target.Returns("ExistingUser,5");
        var existingLevel = new ArcadeLevel { Id = "existinguser", Level = 2 };
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingLevel);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(existingLevel.Level, Is.EqualTo(5));
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("arcade_level_update", "existinguser", 5);
        await _arcadeLevelsDbSet.DidNotReceive().AddAsync(Arg.Any<ArcadeLevel>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleExceptionAndReplyError_WhenAddFails()
    {
        // Arrange
        _context.Target.Returns("user,3");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception("Database connection error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_update_error", "Database connection error");
        _context.DidNotReceive().ReplyLocalizedMessage("arcade_level_add", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleExceptionAndReplyError_WhenUpdateFails()
    {
        // Arrange
        _context.Target.Returns("user,3");
        var existingLevel = new ArcadeLevel { Id = "user", Level = 2 };
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingLevel);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception("Update failed"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_update_error", "Update failed");
        _context.DidNotReceive().ReplyLocalizedMessage("arcade_level_update", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizeUsername_ToLowerAlphaNum()
    {
        // Arrange
        _context.Target.Returns("Test-User_123!,5");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _arcadeLevelsDbSet.Received(1).AddAsync(
            Arg.Is<ArcadeLevel>(level => level.Id == "testuser123"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCancellationToken_ToAllDbOperations()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _context.Target.Returns("user,3");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context, cancellationToken);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(cancellationToken);
        await _arcadeLevelsDbSet.Received(1).FindAsync(Arg.Any<object[]>(), cancellationToken);
        await _dbContext.Received(1).SaveChangesAsync(cancellationToken);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_AfterExecution()
    {
        // Arrange
        _context.Target.Returns("user,3");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);
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
        _context.Target.Returns("user,3");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception("Error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldFindExistingLevel_UsingNormalizedUsername()
    {
        // Arrange
        _context.Target.Returns("TestUser,5");
        var existingLevel = new ArcadeLevel { Id = "testuser", Level = 2 };
        _arcadeLevelsDbSet.FindAsync(Arg.Is<object[]>(arr => arr[0].ToString() == "testuser"), Arg.Any<CancellationToken>())
            .Returns(existingLevel);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _arcadeLevelsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => arr.Length == 1 && arr[0].ToString() == "testuser"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleWhitespaceInInput()
    {
        // Arrange
        _context.Target.Returns(" user , 5 ");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _arcadeLevelsDbSet.Received(1).AddAsync(
            Arg.Is<ArcadeLevel>(level => level.Id == "user" && level.Level == 5),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyAddMessage_BeforeSavingChanges()
    {
        // Arrange
        _context.Target.Returns("user,3");
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((ArcadeLevel)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var callOrder = new List<string>();
        _context.When(x => x.ReplyLocalizedMessage(Arg.Any<string>(), Arg.Any<object[]>()))
            .Do(_ => callOrder.Add("reply"));
        _dbContext.When(x => x.SaveChangesAsync(Arg.Any<CancellationToken>()))
            .Do(_ => callOrder.Add("save"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(callOrder, Is.EqualTo(new[] { "reply", "save" }));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSaveChanges_AfterReplyingUpdateMessage()
    {
        // Arrange
        _context.Target.Returns("user,5");
        var existingLevel = new ArcadeLevel { Id = "user", Level = 2 };
        _arcadeLevelsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingLevel);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var callOrder = new List<string>();
        _dbContext.When(x => x.SaveChangesAsync(Arg.Any<CancellationToken>()))
            .Do(_ => callOrder.Add("save"));
        _context.When(x => x.ReplyLocalizedMessage(Arg.Any<string>(), Arg.Any<object[]>()))
            .Do(_ => callOrder.Add("reply"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(callOrder, Is.EqualTo(new[] { "save", "reply" }));
    }
}