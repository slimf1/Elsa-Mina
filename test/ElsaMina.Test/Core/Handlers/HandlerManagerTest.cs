using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.DependencyInjection;
using NSubstitute;

namespace ElsaMina.Test.Core.Handlers;

public class HandlerManagerTests
{
    private IDependencyContainerService _mockContainerService;
    private HandlerManager _handlerManager;

    [SetUp]
    public void SetUp()
    {
        _mockContainerService = Substitute.For<IDependencyContainerService>();
        _handlerManager = new HandlerManager(_mockContainerService);
    }

    [Test]
    public void Test_Initialize_ShouldPopulateHandlers_WhenHandlersAreResolved()
    {
        // Arrange
        var handler1 = Substitute.For<IHandler>();
        handler1.Identifier.Returns("Handler1");
        var handler2 = Substitute.For<IHandler>();
        handler2.Identifier.Returns("Handler2");

        _mockContainerService.Resolve<IEnumerable<IHandler>>()
            .Returns(new[] { handler1, handler2 });

        // Act
        _handlerManager.Initialize();

        // Assert
        Assert.That(_handlerManager.IsInitialized, Is.True);
    }

    [Test]
    public void Test_Initialize_ShouldSetIsInitializedTrue_WhenCalled()
    {
        // Arrange
        var handler = Substitute.For<IHandler>();
        _mockContainerService.Resolve<IEnumerable<IHandler>>()
            .Returns(new[] { handler });

        // Act
        _handlerManager.Initialize();

        // Assert
        Assert.That(_handlerManager.IsInitialized, Is.True);
    }

    [Test]
    public async Task Test_HandleMessage_ShouldInvokeOnMessageReceivedOnEnabledHandlers_WhenCalled()
    {
        // Arrange
        var handler1 = Substitute.For<IHandler>();
        handler1.IsEnabled.Returns(true);
        var handler2 = Substitute.For<IHandler>();
        handler2.IsEnabled.Returns(false);
        var handler3 = Substitute.For<IHandler>();
        handler3.IsEnabled.Returns(true);

        handler1.Identifier.Returns("Handler1");
        handler2.Identifier.Returns("Handler2");
        handler3.Identifier.Returns("Handler3");

        _mockContainerService.Resolve<IEnumerable<IHandler>>()
            .Returns(new[] { handler1, handler2, handler3 });
        _handlerManager.Initialize();

        var parts = new[] { "MessagePart1", "MessagePart2" };
        var roomId = "Room1";

        // Act
        await _handlerManager.HandleMessage(parts, roomId);

        // Assert
        await handler1.Received(1).OnMessageReceived(parts, roomId);
        await handler2.DidNotReceive().OnMessageReceived(Arg.Any<string[]>(), Arg.Any<string>());
        await handler3.Received(1).OnMessageReceived(parts, roomId);
    }
}