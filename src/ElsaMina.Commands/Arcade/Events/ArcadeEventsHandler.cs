using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Arcade.Events;

public class ArcadeEventsHandler : Handler
{
    private const string WEBHOOK_USERNAME = "Elsa Mina";
    private const string WEBHOOK_AVATAR_URL = "https://play.pokemonshowdown.com/sprites/trainers/lusamine.png";
    private const string NOTIFICATION_TITLE = "Event Notification";
    private const int NOTIFICATION_COLOR = 3066993;

    private static readonly Regex EVENT_REGEX =
        new("""<div class="broadcast-blue"><b>The "(.*?)" roomevent has started!</b></div>""", RegexOptions.Compiled,
            Constants.REGEX_MATCH_TIMEOUT);

    private static readonly Regex INFOBOX_REGEX =
        new(@"<td>(.*?)</td><td>(.*?)</td><td><time>.*?</time></td>",
            RegexOptions.Compiled | RegexOptions.Singleline,
            Constants.REGEX_MATCH_TIMEOUT);

    private static readonly Regex HTML_TAG_REGEX =
        new(@"<.*?>", RegexOptions.Compiled | RegexOptions.Singleline, Constants.REGEX_MATCH_TIMEOUT);

    private readonly ConcurrentDictionary<string, bool> _pendingEvents = new();
    private readonly IHttpService _httpService;
    private readonly IConfiguration _configuration;
    private readonly IBot _bot;

    public ArcadeEventsHandler(IHttpService httpService, IConfiguration configuration, IBot bot)
    {
        _httpService = httpService;
        _configuration = configuration;
        _bot = bot;
    }

    public override async Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        if (roomId != "arcade" || parts.Length < 3 || parts[1] != "raw")
        {
            return;
        }

        var rawMessage = parts[2].Trim();

        var eventStartMatch = EVENT_REGEX.Match(rawMessage);
        if (eventStartMatch.Success)
        {
            var eventName = eventStartMatch.Groups[1].Value;
            _pendingEvents[eventName] = true;
            _bot.Say("arcade", $"!events view {eventName}");
            return;
        }

        if (!rawMessage.Contains("infobox-limited"))
        {
            return;
        }

        var detailsMatch = INFOBOX_REGEX.Match(rawMessage);
        if (!detailsMatch.Success)
        {
            return;
        }

        var detailEventName = detailsMatch.Groups[1].Value.Trim();
        if (!_pendingEvents.TryRemove(detailEventName, out _))
        {
            return;
        }

        var rawDescription = detailsMatch.Groups[2].Value.Trim();
        var eventDescription = HTML_TAG_REGEX.Replace(rawDescription, string.Empty);

        var webhookUrl = _configuration.ArcadeWebhookUrl;
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            Log.Error("Arcade webhook url is missing.");
            return;
        }

        try
        {
            var embedDescription =
                $"L'événement '{detailEventName}' a commencé dans la room [Arcade](https://play.pokemonshowdown.com/arcade) !\n\n**Description:** {eventDescription}";

            var body = new ArcadeEventWebhookBody
            {
                AvatarUrl = WEBHOOK_AVATAR_URL,
                Username = WEBHOOK_USERNAME,
                Content = string.Empty,
                Embeds =
                [
                    new ArcadeEventWebhookEmbed
                    {
                        Title = NOTIFICATION_TITLE,
                        Color = NOTIFICATION_COLOR,
                        Description = embedDescription
                    }
                ]
            };

            var response = await _httpService.PostJsonAsync<ArcadeEventWebhookBody, object>(webhookUrl, body,
                cancellationToken: cancellationToken);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("Sent arcade announce via webhook successfully.");
            }
            else
            {
                Log.Error("Error while sending arcade announce via webhook : received = {0}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while sending arcade announce via webhook");
        }
    }
}
