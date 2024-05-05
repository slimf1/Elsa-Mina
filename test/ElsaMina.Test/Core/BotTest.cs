using ElsaMina.Core;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.Parsers;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.System;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Services.UserDetails;
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
    private IParsersManager _parsersManager;
    private IUserDetailsManager _userDetailsManager;
    private ISystemService _systemService;
    private ITemplatesManager _templatesManager;

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
        _parsersManager = Substitute.For<IParsersManager>();
        _userDetailsManager = Substitute.For<IUserDetailsManager>();
        _systemService = Substitute.For<ISystemService>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        
        _bot = new Bot(_client, _configurationManager, _clockService, _roomsManager,
            _formatsManager, _loginService, _parsersManager, _userDetailsManager, _systemService, _templatesManager);
    }

    [TearDown]
    public void TearDown()
    {
        _bot.Dispose();
    }

    [Test]
    public async Task Test_Start_ShouldConnectAndPreCompileTemplates()
    {
        // Act
        await _bot.Start();
        
        // Assert
        await _templatesManager.Received(1).PreCompileTemplates();
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
    public async Task Test_HandleReceivedMessage_ShouldInitializeParsers_WhenParsersAreNotInitialized()
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