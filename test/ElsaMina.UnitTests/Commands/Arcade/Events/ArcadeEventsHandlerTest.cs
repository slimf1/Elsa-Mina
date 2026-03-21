using System.Net;
using ElsaMina.Commands.Arcade.Events;
using ElsaMina.Core;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Arcade.Events;

public class ArcadeEventsHandlerTests
{
    private const string EVENT_START_MESSAGE =
        """<div class="broadcast-blue"><b>The "TestEvent" roomevent has started!</b></div>""";
    private const string INFOBOX_MESSAGE =
        """infobox-limited <td>TestEvent</td><td>Some description</td><td><time>2023-01-01</time></td>""";

    private ArcadeEventsHandler _handler;
    private IHttpService _httpService;
    private IConfiguration _configuration;
    private IBot _bot;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _configuration = Substitute.For<IConfiguration>();
        _bot = Substitute.For<IBot>();

        var httpResponse = Substitute.For<IHttpResponse<object>>();
        httpResponse.StatusCode.Returns(HttpStatusCode.NoContent);
        _httpService
            .PostJsonAsync<ArcadeEventWebhookBody, object>(
                Arg.Any<string>(), Arg.Any<ArcadeEventWebhookBody>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(httpResponse));

        _configuration.ArcadeWebhookUrl.Returns("http://webhook.url");

        var eventRoleMappingService = Substitute.For<IEventRoleMappingService>();
        _handler = new ArcadeEventsHandler(_httpService, _configuration, _bot, eventRoleMappingService);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldReturn_WhenRoomIdIsNotArcade()
    {
        var parts = new[] { "", "raw", EVENT_START_MESSAGE };

        await _handler.HandleReceivedMessageAsync(parts, "general");

        _bot.DidNotReceiveWithAnyArgs().Say(default, default);
        await _httpService.DidNotReceiveWithAnyArgs()
            .PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldReturn_WhenPartsAreTooShort()
    {
        var parts = new[] { "", "raw" };

        await _handler.HandleReceivedMessageAsync(parts, "arcade");

        _bot.DidNotReceiveWithAnyArgs().Say(default, default);
        await _httpService.DidNotReceiveWithAnyArgs()
            .PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldReturn_WhenMessageTypeIsNotRaw()
    {
        var parts = new[] { "", "other", EVENT_START_MESSAGE };

        await _handler.HandleReceivedMessageAsync(parts, "arcade");

        _bot.DidNotReceiveWithAnyArgs().Say(default, default);
        await _httpService.DidNotReceiveWithAnyArgs()
            .PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldSendEventsViewCommand_WhenEventStartDetected()
    {
        var parts = new[] { "", "raw", EVENT_START_MESSAGE };

        await _handler.HandleReceivedMessageAsync(parts, "arcade");

        _bot.Received(1).Say("arcade", "!events view TestEvent");
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotSendWebhook_WhenOnlyEventStartReceived()
    {
        var parts = new[] { "", "raw", EVENT_START_MESSAGE };

        await _handler.HandleReceivedMessageAsync(parts, "arcade");

        await _httpService.DidNotReceiveWithAnyArgs()
            .PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldSendWebhook_WhenEventDetailsReceivedAfterStart()
    {
        var startParts = new[] { "", "raw", EVENT_START_MESSAGE };
        var infoboxParts = new[] { "", "raw", INFOBOX_MESSAGE };

        await _handler.HandleReceivedMessageAsync(startParts, "arcade");
        await _handler.HandleReceivedMessageAsync(infoboxParts, "arcade");

        await _httpService.Received(1).PostJsonAsync<ArcadeEventWebhookBody, object>(
            "http://webhook.url",
            Arg.Is<ArcadeEventWebhookBody>(body =>
                body.Username == "Elsa Mina" &&
                body.AvatarUrl == "https://play.pokemonshowdown.com/sprites/trainers/lusamine.png" &&
                body.Content == string.Empty &&
                body.Embeds.Count == 1 &&
                body.Embeds[0].Title == "Event Notification" &&
                body.Embeds[0].Color == 3066993 &&
                body.Embeds[0].Description.Contains("TestEvent") &&
                body.Embeds[0].Description.Contains("Some description")),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotSendWebhook_WhenEventNameIsNotPending()
    {
        var infoboxParts = new[] { "", "raw", INFOBOX_MESSAGE };

        await _handler.HandleReceivedMessageAsync(infoboxParts, "arcade");

        await _httpService.DidNotReceiveWithAnyArgs()
            .PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldStripHtmlTagsFromDescription_WhenEventDetailsReceived()
    {
        var startParts = new[] { "", "raw", EVENT_START_MESSAGE };
        var infoboxParts = new[]
        {
            "", "raw",
            """infobox-limited <td>TestEvent</td><td>Some <b>bold</b> and <i>italic</i> text</td><td><time>2023-01-01</time></td>"""
        };

        await _handler.HandleReceivedMessageAsync(startParts, "arcade");
        await _handler.HandleReceivedMessageAsync(infoboxParts, "arcade");

        await _httpService.Received(1).PostJsonAsync<ArcadeEventWebhookBody, object>(
            Arg.Any<string>(),
            Arg.Is<ArcadeEventWebhookBody>(body =>
                body.Embeds[0].Description.Contains("Some bold and italic text") &&
                !body.Embeds[0].Description.Contains("<b>") &&
                !body.Embeds[0].Description.Contains("<i>")),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotSendWebhook_WhenInboxLimitedButRegexDoesNotMatch()
    {
        var startParts = new[] { "", "raw", EVENT_START_MESSAGE };
        var infoboxParts = new[] { "", "raw", "infobox-limited <p>no table structure here</p>" };

        await _handler.HandleReceivedMessageAsync(startParts, "arcade");
        await _handler.HandleReceivedMessageAsync(infoboxParts, "arcade");

        await _httpService.DidNotReceiveWithAnyArgs()
            .PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotSendWebhook_WhenWebhookUrlIsMissing()
    {
        _configuration.ArcadeWebhookUrl.Returns(string.Empty);
        var startParts = new[] { "", "raw", EVENT_START_MESSAGE };
        var infoboxParts = new[] { "", "raw", INFOBOX_MESSAGE };

        await _handler.HandleReceivedMessageAsync(startParts, "arcade");
        await _handler.HandleReceivedMessageAsync(infoboxParts, "arcade");

        await _httpService.DidNotReceiveWithAnyArgs()
            .PostJsonAsync<ArcadeEventWebhookBody, object>(default, default);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotThrow_WhenHttpServiceThrowsException()
    {
        var startParts = new[] { "", "raw", EVENT_START_MESSAGE };
        var infoboxParts = new[] { "", "raw", INFOBOX_MESSAGE };

        _httpService
            .PostJsonAsync<ArcadeEventWebhookBody, object>(
                Arg.Any<string>(), Arg.Any<ArcadeEventWebhookBody>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        await _handler.HandleReceivedMessageAsync(startParts, "arcade");
        Assert.DoesNotThrowAsync(() => _handler.HandleReceivedMessageAsync(infoboxParts, "arcade"));
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldOnlySendWebhookOnce_WhenEventDetailsReceivedTwice()
    {
        var startParts = new[] { "", "raw", EVENT_START_MESSAGE };
        var infoboxParts = new[] { "", "raw", INFOBOX_MESSAGE };

        await _handler.HandleReceivedMessageAsync(startParts, "arcade");
        await _handler.HandleReceivedMessageAsync(infoboxParts, "arcade");
        await _handler.HandleReceivedMessageAsync(infoboxParts, "arcade");

        await _httpService.Received(1).PostJsonAsync<ArcadeEventWebhookBody, object>(
            Arg.Any<string>(), Arg.Any<ArcadeEventWebhookBody>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
