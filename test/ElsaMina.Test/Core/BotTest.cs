using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Start;
using ElsaMina.Core.Services.System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

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
    private IStartManager _startManager;

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
        _startManager = Substitute.For<IStartManager>();

        _bot = new Bot(_client, _configurationManager, _clockService, _roomsManager,
            _formatsManager, _loginService, _handlerManager, _systemService, _startManager);
    }

    [Test]
    public async Task Test_Start_ShouldConnectAndPreCompileTemplates()
    {
        // Act
        await _bot.Start();

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

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldLogin_WhenChallstrHasBeenReceived()
    {
        // Arrange
        const string message = "|challstr|4|nonce";
        _configurationManager.Configuration.Returns(new Configuration
        {
            Name = "LeBot"
        });
        _loginService.Login("4|nonce").Returns(new LoginResponseDto
        {
            Assertion = "assertion",
            CurrentUser = new CurrentUserDto
            {
                IsLoggedIn = true,
                UserId = "lebot",
                Username = "LeBot"
            }
        });

        // Act
        await _bot.HandleReceivedMessage(message);

        // Assert
        _systemService.DidNotReceive().Kill();
        _client.Received(1).Send("|/trn LeBot,0,assertion");
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldKill_WhenLoginFails()
    {
        // Arrange
        const string message = "|challstr|4|nonce";
        _configurationManager.Configuration.Returns(new Configuration
        {
            Name = "LeBot"
        });
        _loginService.Login("4|nonce").ReturnsNull();

        // Act
        await _bot.HandleReceivedMessage(message);

        // Assert
        _systemService.Received(1).Kill();
        _client.DidNotReceive().Send(Arg.Any<string>());
    }
    
    [Test]
    public async Task Test_HandleReceivedMessage_ShouldJoinRooms_WhenConnectionHasBeenVeritified()
    {
        // Arrange
        const string message = "|updateuser|+LeBot|1|1|{}";
        _configurationManager.Configuration.Returns(new Configuration
        {
            Name = "LeBot",
            Rooms = ["botdev", "franais", "lobby"],
            RoomBlacklist = ["lobby"]
        });

        // Act
        await _bot.HandleReceivedMessage(message);

        // Assert
        _client.Received(1).Send("|/join botdev");
        _client.Received(1).Send("|/join franais");
        _client.DidNotReceive().Send("|/join lobby");
    }
    
    [Test]
    public async Task Test_HandleReceivedMessage_ShouldDoNothing_WhenIsConnectedAsGuest()
    {
        // Arrange
        const string message = "|updateuser| Guest 123|1|1|{}";
        _configurationManager.Configuration.Returns(new Configuration
        {
            Name = "LeBot",
            Rooms = ["botdev", "franais", "lobby"],
            RoomBlacklist = ["lobby"]
        });

        // Act
        await _bot.HandleReceivedMessage(message);

        // Assert
        _client.DidNotReceive().Send(Arg.Any<string>());
    }
}