using ElsaMina.Commands.Development.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Development.Commands;

public class RunningCommandsTests
{
    private ICommandExecutor _commandExecutor;
    private IContext _context;
    private RunningCommands _command;

    [SetUp]
    public void SetUp()
    {
        _commandExecutor = Substitute.For<ICommandExecutor>();
        _context = Substitute.For<IContext>();
        _command = new RunningCommands(_commandExecutor);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyEmptyHtml_WhenNoRunningCommands()
    {
        // Arrange
        _commandExecutor.RunningCommands.Returns(Array.Empty<RunningCommand>());

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyHtml(string.Empty);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyRunningCommands_WhenCommandsExist()
    {
        // Arrange
        var executionId = Guid.NewGuid();
        var runningContext = Substitute.For<IContext>();
        var sender = Substitute.For<IUser>();
        sender.UserId.Returns("user123");
        runningContext.Sender.Returns(sender);
        runningContext.RoomId.Returns("room1");

        var running = new RunningCommand(
            executionId,
            "timer",
            runningContext,
            new CancellationTokenSource(),
            Task.CompletedTask);

        _commandExecutor.RunningCommands.Returns(new[] { running });

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyHtml($"timer - id={executionId}, room=room1, sender=user123<br />");
    }
}
