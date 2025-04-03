using ElsaMina.Core;
using ElsaMina.Core.Services.DependencyInjection;
using NSubstitute;

namespace ElsaMina.Test.Core;

public class BotLifecycleManagerTest
{
    private IDependencyContainerService _dependencyContainerService;
    private BotLifecycleManager _botLifecycleManager;
    private IBotLifecycleHandler _handler1;
    private IBotLifecycleHandler _handler2;

    [SetUp]
    public void SetUp()
    {
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _handler1 = Substitute.For<IBotLifecycleHandler>();
        _handler2 = Substitute.For<IBotLifecycleHandler>();
        _handler1.Priority.Returns(1);
        _handler2.Priority.Returns(2);

        _dependencyContainerService.Resolve<IEnumerable<IBotLifecycleHandler>>()
            .Returns((List<IBotLifecycleHandler>) [_handler1, _handler2]);

        _botLifecycleManager = new BotLifecycleManager(_dependencyContainerService);
    }

    [Test]
    public void OnStart_ShouldInvokeOnStartOnAllHandlersInOrder()
    {
        // Act
        _botLifecycleManager.OnStart();

        // Assert
        Received.InOrder(() =>
        {
            _handler2.OnStart();
            _handler1.OnStart();
        });
    }

    [Test]
    public void OnReconnect_ShouldInvokeOnReconnectOnAllHandlersInOrder()
    {
        // Act
        _botLifecycleManager.OnReconnect();

        // Assert
        Received.InOrder(() =>
        {
            _handler2.OnReconnect();
            _handler1.OnReconnect();
        });
    }

    [Test]
    public void OnDisconnect_ShouldInvokeOnDisconnectOnAllHandlersInOrder()
    {
        // Act
        _botLifecycleManager.OnDisconnect();

        // Assert
        Received.InOrder(() =>
        {
            _handler2.OnDisconnect();
            _handler1.OnDisconnect();
        });
    }
}