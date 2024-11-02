using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.CustomColors;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Services.System;
using ElsaMina.Core.Services.Templates;
using NSubstitute;

namespace ElsaMina.Test.Core;

public class BotTest
{
    private IClient _client;
    private IConfigurationManager _configurationManager;
    private IClockService _clockService;
    private IRoomsManager _roomsManager;
    private IFormatsManager _formatsManager;
    private ILoginService _loginService;
    private IHandlerManager _handlerManager;
    private ISystemService _systemService;
    private ITemplatesManager _templatesManager;
    private ICommandExecutor _commandExecutor;
    private ICustomColorsManager _customColorsManager;
    private IRoomUserDataService _roomUserDataService;

    private Bot _bot;
    
    [SetUp]
    public void SetUp()
    {
        _client = Substitute.For<IClient>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _clockService = Substitute.For<IClockService>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _formatsManager = Substitute.For<IFormatsManager>();
        _loginService = Substitute.For<ILoginService>();
        _handlerManager = Substitute.For<IHandlerManager>();
        _systemService = Substitute.For<ISystemService>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _commandExecutor = Substitute.For<ICommandExecutor>();
        _customColorsManager = Substitute.For<ICustomColorsManager>();
        _roomUserDataService = Substitute.For<IRoomUserDataService>();
        
        _bot = new Bot(_client, _configurationManager, _clockService, _roomsManager,
            _formatsManager, _loginService, _handlerManager, _systemService, _templatesManager, _commandExecutor,
            _customColorsManager, _roomUserDataService);
    }

    [Test]
    public async Task Test_Start_ShouldConnectAndPreCompileTemplates()
    {
        // Act
        await _bot.Start();
        
        // Assert
        await _templatesManager.Received(1).CompileTemplates();
        await _commandExecutor.Received(1).OnBotStartUp();
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