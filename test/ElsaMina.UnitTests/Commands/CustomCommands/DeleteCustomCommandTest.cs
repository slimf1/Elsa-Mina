using ElsaMina.Commands.CustomCommands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.CustomCommands;

public class DeleteCustomCommandTest
{
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private DbSet<AddedCommand> _addedCommandsDbSet;
    private IContext _context;
    private DeleteCustomCommand _command;

    [SetUp]
    public void SetUp()
    {
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContext = Substitute.For<BotDbContext>();
        _addedCommandsDbSet = Substitute.For<DbSet<AddedCommand>>();
        _context = Substitute.For<IContext>();

        _dbContext.AddedCommands.Returns(_addedCommandsDbSet);
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_dbContext);

        _command = new DeleteCustomCommand(_dbContextFactory);
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
        Assert.That(_command.HelpMessageKey, Is.EqualTo("deletecommand_help"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldDeleteCommandAndReplySuccess_WhenCommandExists()
    {
        // Arrange
        var existingCommand = new AddedCommand { Id = "commandtodelete", RoomId = "room1" };
        _context.Target.Returns("CommandToDelete");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _dbContext.AddedCommands.Received(1).Remove(existingCommand);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("deletecommand_success", "commandtodelete");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNotFound_WhenCommandDoesNotExist()
    {
        // Arrange
        _context.Target.Returns("nonexistent");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("deletecommand_not_found", "nonexistent");
        _dbContext.AddedCommands.DidNotReceive().Remove(Arg.Any<AddedCommand>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFailureMessage_WhenFindAsyncThrowsException()
    {
        // Arrange
        _context.Target.Returns("command");
        _context.RoomId.Returns("room1");
        var exceptionMessage = "Database connection error";
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Throws(new Exception(exceptionMessage));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("deletecommand_failure", exceptionMessage);
        _context.DidNotReceive().ReplyLocalizedMessage("deletecommand_success", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFailureMessage_WhenSaveChangesThrowsException()
    {
        // Arrange
        var existingCommand = new AddedCommand { Id = "command", RoomId = "room1" };
        _context.Target.Returns("command");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        var exceptionMessage = "Save failed";
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception(exceptionMessage));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("deletecommand_failure", exceptionMessage);
        _context.DidNotReceive().ReplyLocalizedMessage("deletecommand_success", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimAndLowercaseCommandId()
    {
        // Arrange
        var existingCommand = new AddedCommand { Id = "testcommand", RoomId = "room1" };
        _context.Target.Returns("  TestCommand  ");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr =>
                arr.Length == 2 &&
                arr[0].ToString() == "testcommand"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldFindCommand_UsingCommandIdAndRoomId()
    {
        // Arrange
        _context.Target.Returns("command");
        _context.RoomId.Returns("specific-room");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr =>
                arr.Length == 2 &&
                arr[0].ToString() == "command" &&
                arr[1].ToString() == "specific-room"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCancellationToken_ToAllDbOperations()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var existingCommand = new AddedCommand { Id = "command", RoomId = "room1" };
        _context.Target.Returns("command");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context, cancellationToken);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(cancellationToken);
        await _addedCommandsDbSet.Received(1).FindAsync(Arg.Any<object[]>(), cancellationToken);
        await _dbContext.Received(1).SaveChangesAsync(cancellationToken);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_AfterSuccessfulExecution()
    {
        // Arrange
        var existingCommand = new AddedCommand { Id = "command", RoomId = "room1" };
        _context.Target.Returns("command");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_WhenCommandNotFound()
    {
        // Arrange
        _context.Target.Returns("nonexistent");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_EvenWhenExceptionOccurs()
    {
        // Arrange
        _context.Target.Returns("command");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldRemoveCommand_BeforeSavingChanges()
    {
        // Arrange
        var existingCommand = new AddedCommand { Id = "command", RoomId = "room1" };
        _context.Target.Returns("command");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var callOrder = new List<string>();
        _dbContext.AddedCommands.When(x => x.Remove(Arg.Any<AddedCommand>()))
            .Do(_ => callOrder.Add("remove"));
        _dbContext.When(x => x.SaveChangesAsync(Arg.Any<CancellationToken>()))
            .Do(_ => callOrder.Add("save"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(callOrder, Is.EqualTo(new[] { "remove", "save" }));
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotCallSaveChanges_WhenCommandNotFound()
    {
        // Arrange
        _context.Target.Returns("nonexistent");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleEmptyTarget()
    {
        // Arrange
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("deletecommand_not_found", string.Empty);
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseCorrectRoomId_FromContext()
    {
        // Arrange
        _context.Target.Returns("command");
        _context.RoomId.Returns("myroom");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => arr[1].ToString() == "myroom"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldOnlyRemoveSpecificCommand()
    {
        // Arrange
        var commandToDelete = new AddedCommand { Id = "command1", RoomId = "room1" };
        _context.Target.Returns("command1");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(commandToDelete);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _dbContext.AddedCommands.Received(1).Remove(
            Arg.Is<AddedCommand>(c => c.Id == "command1" && c.RoomId == "room1"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleWhitespaceInTarget()
    {
        // Arrange
        var existingCommand = new AddedCommand { Id = "command", RoomId = "room1" };
        _context.Target.Returns("   command   ");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr => arr[0].ToString() == "command"),
            Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("deletecommand_success", "command");
    }
}