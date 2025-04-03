using System.Globalization;
using ElsaMina.Commands.Arcade;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.Test.Commands.Arcade;

public class ArcadeEventsHandlerTests
{
    private ArcadeEventsHandler _handler;
    private IHttpService _httpService;
    private IConfigurationManager _configurationManager;
    private IResourcesService _resourcesService;
    private IRoomsManager _roomsManager;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _resourcesService = Substitute.For<IResourcesService>();
        _roomsManager = Substitute.For<IRoomsManager>();

        _handler = new ArcadeEventsHandler(
            _httpService,
            _configurationManager,
            _resourcesService,
            _roomsManager);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldReturn_WhenRoomIdIsNotArcade()
    {
        // Arrange
        var parts = new[] { "", "raw", "<div class=\"broadcast-blue\"><b>The \"TestEvent\" roomevent has started!</b></div>" };

        // Act
        await _handler.HandleReceivedMessageAsync(parts, "general");

        // Assert
        await _httpService.DidNotReceiveWithAnyArgs().PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldReturn_WhenPartsAreInvalid()
    {
        // Arrange
        var parts = new[] { "", "other" };

        // Act
        await _handler.HandleReceivedMessageAsync(parts, "arcade");

        // Assert
        await _httpService.DidNotReceiveWithAnyArgs().PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldLogError_WhenWebhookUrlIsMissing()
    {
        // Arrange
        _configurationManager.Configuration.ArcadeWebhookUrl.Returns(string.Empty);
        var parts = new[] { "", "raw", "<div class=\"broadcast-blue\"><b>The \"TestEvent\" roomevent has started!</b></div>" };

        // Act
        await _handler.HandleReceivedMessageAsync(parts, "arcade");

        // Assert
        await _httpService.DidNotReceiveWithAnyArgs().PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldSendWebhook_WhenValidEventOccurs()
    {
        // Arrange
        var webhookUrl = "http://webhook.url";
        var culture = new CultureInfo("en-US");
        _configurationManager.Configuration.ArcadeWebhookUrl.Returns(webhookUrl);
        var room = Substitute.For<IRoom>();
        room.Culture.Returns(culture);
        _roomsManager.GetRoom("arcade").Returns(room);

        var eventName = "TestEvent";
        var messageTemplate = "Event {0} has started!";
        _resourcesService.GetString("arcade_event_announce", culture).Returns(messageTemplate);
        var parts = new[] { "", "raw", $"<div class=\"broadcast-blue\"><b>The \"{eventName}\" roomevent has started!</b></div>" };

        // Act
        await _handler.HandleReceivedMessageAsync(parts, "arcade");

        // Assert
        await _httpService.Received(1).PostJsonAsync<ArcadeEventWebhookBody, object>(webhookUrl, Arg.Is<ArcadeEventWebhookBody>(body =>
            body.Username == "Elsa Mina" &&
            body.AvatarUrl == "https://play.pokemonshowdown.com/sprites/trainers/lusamine.png" &&
            body.Embeds.Count == 1 &&
            body.Embeds[0].Title == "Notif Event" &&
            body.Embeds[0].Description == string.Format(messageTemplate, eventName)));
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldLogError_WhenHttpServiceThrowsException()
    {
        // Arrange
        var webhookUrl = "http://webhook.url";
        _configurationManager.Configuration.ArcadeWebhookUrl.Returns(webhookUrl);
        var room = Substitute.For<IRoom>();
        var culture = new CultureInfo("en-US");
        room.Culture.Returns(culture);
        _roomsManager.GetRoom("arcade").Returns(room);

        var eventName = "TestEvent";
        var messageTemplate = "Event {0} has started!";
        _resourcesService.GetString("arcade_event_announce", culture).Returns(messageTemplate);
        var parts = new[] { "", "raw", $"<div class=\"broadcast-blue\"><b>The \"{eventName}\" roomevent has started!</b></div>" };

        _httpService
            .When(x => x.PostJsonAsync<ArcadeEventWebhookBody, object>(webhookUrl, Arg.Any<ArcadeEventWebhookBody>()))
            .Throw(new HttpRequestException("Network error"));

        // Act
        await _handler.HandleReceivedMessageAsync(parts, "arcade");

        // Assert
        await _httpService.Received(1).PostJsonAsync<ArcadeEventWebhookBody, object>(webhookUrl, Arg.Any<ArcadeEventWebhookBody>());
    }
}
