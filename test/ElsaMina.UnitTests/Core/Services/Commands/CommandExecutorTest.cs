using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.AddedCommands;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.DependencyInjection;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Services.Commands;

public class CommandExecutorTest
{
    private IDependencyContainerService _dependencyContainerService;
    private IAddedCommandsManager _addedCommandsManager;
    private CommandExecutor _commandExecutor;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _addedCommandsManager = Substitute.For<IAddedCommandsManager>();
        _context = Substitute.For<IContext>();
        _commandExecutor = new CommandExecutor(_dependencyContainerService, _addedCommandsManager);
    }

    [Test]
    public void Test_GetAllCommands_ShouldReturnAllCommandsFromContainer()
    {
        // Arrange
        var expectedCommands = new List<ICommand>
        {
            Substitute.For<ICommand>(),
            Substitute.For<ICommand>()
        };
        expectedCommands.ElementAt(0).Name.Returns("1");
        expectedCommands.ElementAt(1).Name.Returns("2");
        _dependencyContainerService.GetAllNamedRegistrations<ICommand>().Returns(expectedCommands);

        // Act
        var result = _commandExecutor.GetAllCommands();

        // Assert
        Assert.That(result, Is.EquivalentTo(expectedCommands));
    }

    [Test]
    public void Test_GetAllCommands_ShouldReturnDistinctCommandsByName()
    {
        // Arrange
        var command1 = Substitute.For<ICommand>();
        command1.Name.Returns("cmd1");
        var command2 = Substitute.For<ICommand>();
        command2.Name.Returns("cmd2");
        var duplicateCommand = Substitute.For<ICommand>();
        duplicateCommand.Name.Returns("cmd1");
        _dependencyContainerService.GetAllNamedRegistrations<ICommand>()
            .Returns([command1, command2, duplicateCommand]);

        // Act
        var result = _commandExecutor.GetAllCommands().ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Does.Contain(command1));
        Assert.That(result, Does.Contain(command2));
    }

    [Test]
    public async Task Test_TryExecuteCommandAsync_ShouldRunCommand_WhenRegisteredAndAllowed()
    {
        // Arrange
        var commandName = "testCommand";
        var command = Substitute.For<ICommand>();
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(true);
        _dependencyContainerService.ResolveNamed<ICommand>(commandName).Returns(command);
        _context.HasRankOrHigher(command.RequiredRank).Returns(true);
        command.IsAllowedInPrivateMessage.Returns(true);

        // Act
        await _commandExecutor.TryExecuteCommandAsync(commandName, _context);

        // Assert
        await command.Received(1).RunAsync(_context);
    }

    [Test]
    [TestCase(null, 0)]
    [TestCase("", 0)]
    [TestCase("franais", 1)]
    [TestCase("other", 0)]
    public async Task Test_TryExecuteCommandAsync_ShouldRunCommand_WhenIsNotInRoomRestriction(string roomId,
        int expectedRunCalls)
    {
        // Arrange
        var commandName = "testCommand";
        var command = Substitute.For<ICommand>();
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(true);
        _dependencyContainerService.ResolveNamed<ICommand>(commandName).Returns(command);
        _context.HasRankOrHigher(command.RequiredRank).Returns(true);
        command.RoomRestriction.Returns(["franais"]);
        _context.RoomId.Returns(roomId);

        // Act
        await _commandExecutor.TryExecuteCommandAsync(commandName, _context);

        // Assert
        await command.Received(expectedRunCalls).RunAsync(_context);
    }

    [Test]
    public async Task Test_TryExecuteCommandAsync_ShouldNotRunCommand_WhenNotAllowedByPrivateMessageRestriction()
    {
        // Arrange
        var commandName = "testCommand";
        var command = Substitute.For<ICommand>();
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(true);
        _dependencyContainerService.ResolveNamed<ICommand>(commandName).Returns(command);
        _context.IsPrivateMessage.Returns(true);
        command.IsPrivateMessageOnly.Returns(false);
        command.IsAllowedInPrivateMessage.Returns(false);

        // Act
        await _commandExecutor.TryExecuteCommandAsync(commandName, _context);

        // Assert
        await command.DidNotReceive().RunAsync(_context);
    }

    [Test]
    public async Task Test_TryExecuteCommandAsync_ShouldNotRunCommand_WhenNotWhitelisted()
    {
        // Arrange
        var commandName = "whitelistCommand";
        var command = Substitute.For<ICommand>();
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(true);
        _dependencyContainerService.ResolveNamed<ICommand>(commandName).Returns(command);
        command.IsWhitelistOnly.Returns(true);
        _context.IsSenderWhitelisted.Returns(false);

        // Act
        await _commandExecutor.TryExecuteCommandAsync(commandName, _context);

        // Assert
        await command.DidNotReceive().RunAsync(_context);
    }

    [Test]
    public async Task Test_TryExecuteCommandAsync_ShouldTryAddedCommand_WhenNotRegisteredAndNotPrivateMessage()
    {
        // Arrange
        var commandName = "customCommand";
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(false);
        _context.IsPrivateMessage.Returns(false);

        // Act
        await _commandExecutor.TryExecuteCommandAsync(commandName, _context);

        // Assert
        await _addedCommandsManager.Received(1).TryExecuteAddedCommand(commandName, _context);
    }

    [Test]
    public async Task Test_TryExecuteCommandAsync_ShouldLogError_WhenCommandNotFoundAndIsPrivateMessage()
    {
        // Arrange
        var commandName = "missingCommand";
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(false);
        _context.IsPrivateMessage.Returns(true);

        // Act
        await _commandExecutor.TryExecuteCommandAsync(commandName, _context);

        // Assert
        await _addedCommandsManager.DidNotReceive().TryExecuteAddedCommand(commandName, _context);
    }


    [Test]
    public async Task Test_TryExecuteCommandAsync_ShouldExecuteAddedCommand_WhenCommandNotRegisteredAndNotPrivateMessage()
    {
        // Arrange
        var commandName = "customCommand";
        var context = Substitute.For<IContext>();
        context.IsPrivateMessage.Returns(false);
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(false);

        // Act
        await _commandExecutor.TryExecuteCommandAsync(commandName, context);

        // Assert
        await _addedCommandsManager.Received().TryExecuteAddedCommand(commandName, context);
    }

    [Test]
    public async Task Test_TryExecuteCommandAsync_ShouldDoNothing_WhenCommandNotFoundAndIsPrivateMessage()
    {
        // Arrange
        var commandName = "nonExistentCommand";
        var context = Substitute.For<IContext>();
        context.IsPrivateMessage.Returns(true);
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(false);

        // Act
        await _commandExecutor.TryExecuteCommandAsync(commandName, context);

        // Assert
        await _addedCommandsManager.DidNotReceive().TryExecuteAddedCommand(commandName, context);
        _dependencyContainerService.DidNotReceive().ResolveNamed<ICommand>(commandName);
    }
}