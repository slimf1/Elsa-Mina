using ElsaMina.Core.Client;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using Serilog;

namespace ElsaMina.Test.Core.Bot;

public class BotTest
{
    private ILogger _logger;
    private IClient _client;
    private IConfigurationService _configurationService;
    private IHttpService _httpService;
    private IClockService _clockService;
    private IContextFactory _contextFactory;
    private ICommandExecutor _commandExecutor;
    private IRoomsManager _roomsManager;
    private IFormatsManager _formatsManager;

    private ElsaMina.Core.Bot.Bot _bot;
    
    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
        _client = Substitute.For<IClient>();
        _configurationService = Substitute.For<IConfigurationService>();
        _httpService = Substitute.For<IHttpService>();
        _clockService = Substitute.For<IClockService>();
        _contextFactory = Substitute.For<IContextFactory>();
        _commandExecutor = Substitute.For<ICommandExecutor>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _formatsManager = Substitute.For<IFormatsManager>();
        
        _bot = new ElsaMina.Core.Bot.Bot(_logger, _client,_configurationService,
            _httpService, _clockService, _contextFactory, _commandExecutor,
            _roomsManager, _formatsManager);
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
        _roomsManager.Received(1).InitializeRoom("room", "Room Title",
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
}