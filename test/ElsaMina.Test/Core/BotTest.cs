﻿using ElsaMina.Core;
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

        _bot = new Bot(_client, _clockService, _roomsManager, _handlerManager, _systemService, _startManager);
    }

    [Test]
    public async Task Test_Connect_ShouldStartAndConnect()
    {
        // Act
        await _bot.Connect();

        // Assert
        await _startManager.Received(1).OnStart();
        await _client.Received(1).Connect();
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldInitializeRooms_WhenARoomIsReceived()
    {
        // Arrange
        const string message = ">room\n|init|chat\n|title|Room Title\n|users|5,*Bot,@Mod, Regular,#Ro User,+Voiced\n";

        // Act
        await _bot.HandleReceivedMessage(message);

        // Assert
        var expectedLines = new List<string> { ">room", "|init|chat", "|title|Room Title", "|users|5,*Bot,@Mod, Regular,#Ro User,+Voiced" };
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
        await _handlerManager.Received(1).Initialize();
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
        await _handlerManager.DidNotReceive().Initialize();
        var expectedParts = new[] { "", "c:", "1", "%Earth", "test" };
        await _handlerManager.Received(1).HandleMessage(Arg.Is<string[]>(parts => parts.SequenceEqual(expectedParts)));
    }
}