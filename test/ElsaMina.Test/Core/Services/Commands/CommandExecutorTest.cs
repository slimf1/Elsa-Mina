using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.AddedCommands;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.DependencyInjection;
using NSubstitute;

namespace ElsaMina.Test.Core.Services.Commands;

public class CommandExecutorTest
{
    private IDependencyContainerService _dependencyContainerService;
    private IAddedCommandsManager _addedCommandsManager;
    private CommandExecutor _commandExecutor;

    [SetUp]
    public void SetUp()
    {
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _addedCommandsManager = Substitute.For<IAddedCommandsManager>();
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
        _dependencyContainerService.GetAllRegistrations<ICommand>().Returns(expectedCommands);

        // Act
        var result = _commandExecutor.GetAllCommands();

        // Assert
        Assert.That(result, Is.EquivalentTo(expectedCommands));
    }

    [Test]
    public async Task Test_OnBotStartUp_ShouldInvokeOnBotStartUpOnAllCommands()
    {
        // Arrange
        var commands = new List<ICommand>
        {
            Substitute.For<ICommand>(),
            Substitute.For<ICommand>()
        };
        commands.ElementAt(0).Name.Returns("1");
        commands.ElementAt(1).Name.Returns("2");
        _dependencyContainerService.GetAllRegistrations<ICommand>().Returns(commands);

        // Act
        await _commandExecutor.OnBotStartUp();

        // Assert
        foreach (var command in commands)
        {
            await command.Received().OnBotStartUp();
        }
    }

    [Test]
    public async Task Test_TryExecuteCommand_ShouldExecuteCommand_WhenCommandIsRegistered()
    {
        // Arrange
        var commandName = "sampleCommand";
        var context = Substitute.For<IContext>();
        var command = Substitute.For<ICommand>();
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(true);
        _dependencyContainerService.ResolveNamed<ICommand>(commandName).Returns(command);

        // Act
        await _commandExecutor.TryExecuteCommand(commandName, context);

        // Assert
        await command.Received().Call(context);
    }

    [Test]
    public async Task Test_TryExecuteCommand_ShouldExecuteAddedCommand_WhenCommandNotRegisteredAndNotPrivateMessage()
    {
        // Arrange
        var commandName = "customCommand";
        var context = Substitute.For<IContext>();
        context.IsPrivateMessage.Returns(false);
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(false);

        // Act
        await _commandExecutor.TryExecuteCommand(commandName, context);

        // Assert
        await _addedCommandsManager.Received().TryExecuteAddedCommand(commandName, context);
    }

    [Test]
    public async Task Test_TryExecuteCommand_ShouldLogError_WhenCommandNotFoundAndIsPrivateMessage()
    {
        // Arrange
        var commandName = "nonExistentCommand";
        var context = Substitute.For<IContext>();
        context.IsPrivateMessage.Returns(true);
        _dependencyContainerService.IsRegisteredWithName<ICommand>(commandName).Returns(false);

        // Act
        await _commandExecutor.TryExecuteCommand(commandName, context);

        // Assert
        await _addedCommandsManager.DidNotReceive().TryExecuteAddedCommand(commandName, context);
        _dependencyContainerService.DidNotReceive().ResolveNamed<ICommand>(commandName);
    }
}