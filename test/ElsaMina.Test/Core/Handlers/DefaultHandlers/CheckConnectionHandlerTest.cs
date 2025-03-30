using ElsaMina.Core;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.System;
using NSubstitute;

namespace ElsaMina.Test.Core.Handlers.DefaultHandlers;

public class CheckConnectionHandlerTest
{
    private IConfigurationManager _configurationManager;
    private IClient _client;
    private ISystemService _systemService;

    private CheckConnectionHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _configurationManager = Substitute.For<IConfigurationManager>();
        _client = Substitute.For<IClient>();
        _systemService = Substitute.For<ISystemService>();
        
        _handler = new CheckConnectionHandler(_configurationManager, _client, _systemService);
    }
    
    [Test]
    public async Task Test_HandleReceivedMessage_ShouldJoinRooms_WhenConnectionHasBeenVeritified()
    {
        // Arrange
        string[] message = ["", "updateuser", "+LeBot", "1", "1", "{}"];
        _configurationManager.Configuration.Returns(new Configuration
        {
            Name = "LeBot",
            Rooms = ["botdev", "franais", "lobby"],
            RoomBlacklist = ["lobby"]
        });

        // Act
        await _handler.HandleReceivedMessageAsync(message);

        // Assert
        _client.Received(1).Send("|/join botdev");
        _client.Received(1).Send("|/join franais");
        _client.DidNotReceive().Send("|/join lobby");
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldDoNothing_WhenIsConnectedAsGuest()
    {
        // Arrange
        string[] message = ["", "updateuser", " Guest 123", "1", "1", "{}"];
        _configurationManager.Configuration.Returns(new Configuration
        {
            Name = "LeBot",
            Rooms = ["botdev", "franais", "lobby"],
            RoomBlacklist = ["lobby"]
        });

        // Act
        await _handler.HandleReceivedMessageAsync(message);

        // Assert
        _client.DidNotReceive().Send(Arg.Any<string>());
    }
}