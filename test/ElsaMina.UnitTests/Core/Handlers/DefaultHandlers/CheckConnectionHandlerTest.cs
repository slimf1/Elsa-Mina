using ElsaMina.Core;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.System;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Handlers.DefaultHandlers;

public class CheckConnectionHandlerTest
{
    private IConfiguration _configuration;
    private IClient _client;
    private ISystemService _systemService;

    private CheckConnectionHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _configuration = Substitute.For<IConfiguration>();
        _client = Substitute.For<IClient>();
        _systemService = Substitute.For<ISystemService>();

        _handler = new CheckConnectionHandler(_configuration, _client, _systemService);
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldJoinRooms_WhenConnectionHasBeenVeritified()
    {
        // Arrange
        string[] message = ["", "updateuser", "+LeBot", "1", "1", "{}"];
        _configuration.Name.Returns("LeBot");
        _configuration.Rooms.Returns(["botdev", "franais", "lobby"]);
        _configuration.RoomBlacklist.Returns(["lobby"]);

        // Act
        await _handler.HandleReceivedMessageAsync(message);

        // Assert
        _client.Received(1).Send("|/join botdev");
        _client.Received(1).Send("|/join franais");
        _client.DidNotReceive().Send("|/join lobby");
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldDoNothing_WhenIsConnectedAsGuest()
    {
        // Arrange
        string[] message = ["", "updateuser", " Guest 123", "1", "1", "{}"];
        _configuration.Name.Returns("LeBot");
        _configuration.Rooms.Returns(["botdev", "franais", "lobby"]);
        _configuration.RoomBlacklist.Returns(["lobby"]);

        // Act
        await _handler.HandleReceivedMessageAsync(message);

        // Assert
        _client.DidNotReceive().Send(Arg.Any<string>());
    }

    [Test]
    [TestCase(null, 0)]
    [TestCase("", 0)]
    [TestCase("avy", 1)]
    public async Task Test_HandleReceivedMessageAsync_ShouldSetAvatar_WhenAvatarIsDefinedInConfiguration(string avatar,
        int expectedCalls)
    {
        // Arrange
        string[] message = ["", "updateuser", "+LeBot", "1", "1", "{}"];
        _configuration.Name.Returns("LeBot");
        _configuration.Avatar.Returns(avatar);
        
        // Act
        await _handler.HandleReceivedMessageAsync(message);
        
        // Assert
        _client.Received(expectedCalls).Send($"|/avatar {avatar}");
    }
}