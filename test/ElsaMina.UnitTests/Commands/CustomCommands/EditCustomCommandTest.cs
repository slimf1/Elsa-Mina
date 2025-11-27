using ElsaMina.Commands.CustomCommands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.CustomCommands;

public class EditCustomCommandTest
{
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private DbSet<AddedCommand> _addedCommandsDbSet;
    private IContext _context;
    private EditCustomCommand _command;

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

        _command = new EditCustomCommand(_dbContextFactory);
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
        Assert.That(_command.HelpMessageKey, Is.EqualTo("editcommand_help"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenCommandIsNotFound()
    {
        // Arrange
        _context.Target.Returns("nonexistentcommand, new content");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("editcommand_help");
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldUpdateCommandAndReplySuccess_WhenCommandExists()
    {
        // Arrange
        _context.Target.Returns("existingcommand, updated content");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "existingcommand", Content = "old content", RoomId = "room1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(existingCommand.Content, Is.EqualTo("updated content"));
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("editcommand_success", "existingcommand");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFailureMessage_WhenSaveChangesThrowsException()
    {
        // Arrange
        _context.Target.Returns("existingcommand, new content");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "existingcommand", Content = "old content", RoomId = "room1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        const string exceptionMessage = "Database error";
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception(exceptionMessage));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("editcommand_failure", exceptionMessage);
        _context.DidNotReceive().ReplyLocalizedMessage("editcommand_success", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenFindAsyncThrowsException()
    {
        // Arrange
        _context.Target.Returns("commandid, new content");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Find error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("editcommand_help");
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimAndLowercaseCommandId()
    {
        // Arrange
        _context.Target.Returns("  ExistingCommand  , new content");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "existingcommand", Content = "old content", RoomId = "room1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr =>
                arr.Length == 2 &&
                arr[0].ToString() == "existingcommand"),
            Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("editcommand_success", "existingcommand");
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimContent()
    {
        // Arrange
        _context.Target.Returns("command,   new content with spaces   ");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "command", Content = "old content", RoomId = "room1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(existingCommand.Content, Is.EqualTo("new content with spaces"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleContentWithMultipleCommas()
    {
        // Arrange
        _context.Target.Returns("command,content,with,multiple,commas");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "command", Content = "old content", RoomId = "room1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(existingCommand.Content, Is.EqualTo("content,with,multiple,commas"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldFindCommand_UsingCommandIdAndRoomId()
    {
        // Arrange
        _context.Target.Returns("command, new content");
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
        _context.Target.Returns("command, new content");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "command", Content = "old content", RoomId = "room1" };
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
        _context.Target.Returns("command, new content");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "command", Content = "old content", RoomId = "room1" };
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
        _context.Target.Returns("nonexistent, content");
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
        _context.Target.Returns("command, content");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotCallSaveChanges_WhenCommandNotFound()
    {
        // Arrange
        _context.Target.Returns("nonexistent, content");
        _context.RoomId.Returns("room1");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldUpdateContentInPlace()
    {
        // Arrange
        _context.Target.Returns("command, completely new content");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "command", Content = "original content", RoomId = "room1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var originalReference = existingCommand;

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(existingCommand, Is.SameAs(originalReference));
        Assert.That(existingCommand.Content, Is.EqualTo("completely new content"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleEmptyContentAfterComma()
    {
        // Arrange
        _context.Target.Returns("command,");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "command", Content = "old content", RoomId = "room1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(existingCommand.Content, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseCorrectRoomId_FromContext()
    {
        // Arrange
        _context.Target.Returns("command, content");
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
    public async Task Test_RunAsync_ShouldHandleContentWithLeadingComma()
    {
        // Arrange
        _context.Target.Returns("command,,content starts with comma");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "command", Content = "old content", RoomId = "room1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(existingCommand.Content, Is.EqualTo(",content starts with comma"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotModifyContent_WhenSaveChangesFails()
    {
        // Arrange
        _context.Target.Returns("command, new content");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Id = "command", Content = "old content", RoomId = "room1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception("Save failed"));

        // Act
        await _command.RunAsync(_context);

        // Assert - content was modified but save failed
        Assert.That(existingCommand.Content, Is.EqualTo("new content"));
    }
}