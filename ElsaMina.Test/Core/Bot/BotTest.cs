using ElsaMina.Core.Client;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using FluentAssertions;
using NSubstitute;

namespace ElsaMina.Test.Core.Bot;

public class BotTest
{
    private IClient _client;
    private IConfigurationService _configurationService;
    private IHttpService _httpService;
    private IClockService _clockService;
    private IContextFactory _contextFactory;

    private ElsaMina.Core.Bot.Bot _bot;
    
    [SetUp]
    public void SetUp()
    {
        _client = Substitute.For<IClient>();
        _configurationService = Substitute.For<IConfigurationService>();
        _httpService = Substitute.For<IHttpService>();
        _clockService = Substitute.For<IClockService>();
        _contextFactory = Substitute.For<IContextFactory>();
        
        _bot = new ElsaMina.Core.Bot.Bot(_client, _configurationService, _httpService, _clockService, _contextFactory);
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
    public async Task Test_HandleReceivedMessage_ShouldInitializeRooms()
    {
        // Arrange
        const string message = ">room\n|init|chat\n|title|Room Title\n|users|5,*Bot,@Mod, Regular,#Ro User,+Voiced\n";
        
        // Act
        await _bot.HandleReceivedMessage(message);
        
        // Assert
        _bot.Rooms.Count.Should().Be(1);
        _bot.Rooms.ContainsKey("room").Should().BeTrue();
        _bot.Rooms["room"].Name.Should().Be("Room Title");
        _bot.Rooms["room"].RoomId.Should().Be("room");
        _bot.Rooms["room"].Users.Count.Should().Be(5);
        _bot.Rooms["room"].Users["bot"].Rank.Should().Be('*');
        _bot.Rooms["room"].Users["bot"].Name.Should().Be("Bot");
        _bot.Rooms["room"].Users["mod"].Rank.Should().Be('@');
        _bot.Rooms["room"].Users["mod"].Name.Should().Be("Mod");
        _bot.Rooms["room"].Users["regular"].Rank.Should().Be(' ');
        _bot.Rooms["room"].Users["regular"].Name.Should().Be("Regular");
        _bot.Rooms["room"].Users["rouser"].Rank.Should().Be('#');
        _bot.Rooms["room"].Users["rouser"].Name.Should().Be("Ro User");
        _bot.Rooms["room"].Users["voiced"].Rank.Should().Be('+');
        _bot.Rooms["room"].Users["voiced"].Name.Should().Be("Voiced");
    }
}