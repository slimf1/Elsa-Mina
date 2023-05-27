﻿using ElsaMina.Core.Client;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.Parsers;
using ElsaMina.Core.Services.PrivateMessages;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using Serilog;

namespace ElsaMina.Test.Core.Bot;

public class BotTest
{
    private ILogger _logger;
    private IClient _client;
    private IConfigurationManager _configurationManager;
    private IClockService _clockService;
    private ICommandExecutor _commandExecutor;
    private IRoomsManager _roomsManager;
    private IFormatsManager _formatsManager;
    private ILoginService _loginService;
    private IPmSendersManager _pmSendersManager;
    private IParsersManager _parsersManager;

    private ElsaMina.Core.Bot.Bot _bot;
    
    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
        _client = Substitute.For<IClient>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _clockService = Substitute.For<IClockService>();
        _commandExecutor = Substitute.For<ICommandExecutor>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _formatsManager = Substitute.For<IFormatsManager>();
        _loginService = Substitute.For<ILoginService>();
        _pmSendersManager = Substitute.For<IPmSendersManager>();
        _parsersManager = Substitute.For<IParsersManager>();
        
        _bot = new ElsaMina.Core.Bot.Bot(_logger, _client, _configurationManager, _clockService, _commandExecutor,
            _roomsManager, _formatsManager, _loginService, _pmSendersManager, _parsersManager);
    }

    [TearDown]
    public void TearDown()
    {
        _bot.Dispose();
    }

    [Test]
    public async Task Test_Start_ShouldConnect()
    {
        // Act
        await _bot.Start();
        
        // Assert
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
        var expectedUsers = new List<string> { "*Bot", "@Mod", " Regular", "#Ro User", "+Voiced" };
        await _roomsManager.Received(1).InitializeRoom("room", "Room Title",
            Arg.Is<IEnumerable<string>>(users => users.SequenceEqual(expectedUsers)));
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldInitializeFormats_WhenFormatsAreReceived()
    {
        // Arrange
        const string message = "|formats|,1|S/V Singles|[Gen 9] Random Battle,f|[Gen 9] Unrated Random Battle,b";
        
        // Act
        await _bot.HandleReceivedMessage(message);
        
        // Assert
        _formatsManager.Received(1).ParseFormatsFromReceivedLine(message);
    }
    
    [Test]
    public async Task Test_HandleReceivedMessage_ShouldInitializeParsers_WhenParsersAreNotIntialized()
    {
        // Arrange
        const string message = "|c:|1|%Earth|test";
        _parsersManager.IsInitialized.Returns(false);
        
        // Act
        await _bot.HandleReceivedMessage(message);
        
        // Assert
        _parsersManager.Received(1).Initialize();
        var expectedParts = new[] { "", "c:", "1", "%Earth", "test" };
        await _parsersManager.Received(1).Parse(Arg.Is<string[]>(parts => parts.SequenceEqual(expectedParts)));
    }
    
    [Test]
    public async Task Test_HandleReceivedMessage_ShouldCallParsers_WhenParsersAreInitialized()
    {
        // Arrange
        const string message = "|c:|1|%Earth|test";
        _parsersManager.IsInitialized.Returns(true);
        
        // Act
        await _bot.HandleReceivedMessage(message);
        
        // Assert
        _parsersManager.DidNotReceive().Initialize();
        var expectedParts = new[] { "", "c:", "1", "%Earth", "test" };
        await _parsersManager.Received(1).Parse(Arg.Is<string[]>(parts => parts.SequenceEqual(expectedParts)));
    }
}