using ElsaMina.Commands.Development.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Development.Commands;

public class CancelRunningCommandTests
{
    private ICommandExecutor _commandExecutor;
    private IContext _context;
    private CancelRunningCommand _command;

    [SetUp]
    public void SetUp()
    {
        _commandExecutor = Substitute.For<ICommandExecutor>();
        _context = Substitute.For<IContext>();
        _command = new CancelRunningCommand(_commandExecutor);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyUsage_WhenTargetIsMissing()
    {
        // Arrange
        _context.Target.Returns(string.Empty);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("cancel_command_usage");
        _commandExecutor.DidNotReceive().TryCancel(Arg.Any<Guid>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyUsage_WhenTargetIsNotGuid()
    {
        // Arrange
        _context.Target.Returns("not-a-guid");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("cancel_command_usage");
        _commandExecutor.DidNotReceive().TryCancel(Arg.Any<Guid>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplySuccess_WhenCancelSucceeds()
    {
        // Arrange
        var executionId = Guid.NewGuid();
        _context.Target.Returns(executionId.ToString());
        _commandExecutor.TryCancel(executionId).Returns(true);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("cancel_command_success", executionId);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNotFound_WhenCancelFails()
    {
        // Arrange
        var executionId = Guid.NewGuid();
        _context.Target.Returns(executionId.ToString());
        _commandExecutor.TryCancel(executionId).Returns(false);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("cancel_command_not_found", executionId);
    }
}
