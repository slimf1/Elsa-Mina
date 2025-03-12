using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Start;
using ElsaMina.Core.Services.System;
using NSubstitute;

namespace ElsaMina.Test.Core;

public class BotTest
{
    private IClient _client;
    private IClockService _clockService;
    private IRoomsManager _roomsManager;
    private IHandlerManager _handlerManager;
    private ISystemService _systemService;
    private IStartManager _startManager;
    private IBotLifecycleManager _lifecycleManager;

    private Bot _bot;

    [SetUp]
    public void SetUp()
    {
        _client = Substitute.For<IClient>();
        _clockService = Substitute.For<IClockService>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _handlerManager = Substitute.For<IHandlerManager>();
        _systemService = Substitute.For<ISystemService>();
        _startManager = Substitute.For<IStartManager>();
        _lifecycleManager = Substitute.For<IBotLifecycleManager>();

        _bot = new Bot(_client, _clockService, _roomsManager, _handlerManager, _systemService, _startManager,
            _lifecycleManager);
    }

    [Test]
    public async Task Test_Start_ShouldConnectAndCallLifecycleHandler()
    {
        // Act
        await _bot.Start();

        // Assert
        _lifecycleManager.Received(1).OnStart();
        await _startManager.Received(1).OnStart();
        await _client.Received(1).Connect();
    }

    [Test]
    public void Test_OnReconnect_ShouldCallLifecycleHandler()
    {
        // Act
        _bot.OnReconnect();

        // Assert
        _lifecycleManager.Received(1).OnReconnect();
    }

    [Test]
    public void Test_OnDisconnect_ShouldCallLifecycleHandler()
    {
        // Act
        _bot.OnDisconnect();

        // Assert
        _lifecycleManager.Received(1).OnDisconnect();
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldInitializeRooms_WhenARoomIsReceived()
    {
        // Arrange
        const string message = ">room\n|init|chat\n|title|Room Title\n|users|5,*Bot,@Mod, Regular,#Ro User,+Voiced\n";

        // Act
        await _bot.HandleReceivedMessage(message);

        // Assert
        var expectedLines = new List<string>
            { ">room", "|init|chat", "|title|Room Title", "|users|5,*Bot,@Mod, Regular,#Ro User,+Voiced" };
        await _roomsManager.Received(1).InitializeRoom("room",
            Arg.Is<IEnumerable<string>>(users => users.SequenceEqual(expectedLines)));
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldInitializeHandlers_WhenHandlersAreNotInitialized()
    {
        // Arrange
        const string message = "|c:|1|%Earth|test";
        _handlerManager.IsInitialized.Returns(false);

        // Act
        await _bot.HandleReceivedMessage(message);

        // Assert
        _handlerManager.Received(1).Initialize();
        var expectedParts = new[] { "", "c:", "1", "%Earth", "test" };
        await _handlerManager.Received(1).HandleMessage(Arg.Is<string[]>(parts => parts.SequenceEqual(expectedParts)));
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldCallHandlers_WhenHandlersAreInitialized()
    {
        // Arrange
        const string message = "|c:|1|%Earth|test";
        _handlerManager.IsInitialized.Returns(true);

        // Act
        await _bot.HandleReceivedMessage(message);

        // Assert
        _handlerManager.DidNotReceive().Initialize();
        var expectedParts = new[] { "", "c:", "1", "%Earth", "test" };
        await _handlerManager.Received(1).HandleMessage(Arg.Is<string[]>(parts => parts.SequenceEqual(expectedParts)));
    }
}