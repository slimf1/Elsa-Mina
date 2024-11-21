using System.Net;
using System.Text.RegularExpressions;
using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Arcade;

public class ArcadeEventsHandler : Handler
{
    private const string WEBHOOK_USERNAME = "Elsa Mina";
    private const string WEBHOOK_AVATAR_URL = "https://play.pokemonshowdown.com/sprites/trainers/lusamine.png";
    private const string NOTIFICATION_TITLE = "Notif Event";
    private const int NOTIFICATION_COLOR = 3066993;

    private readonly IHttpService _httpService;
    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;
    private readonly IRoomsManager _roomsManager;

    public ArcadeEventsHandler(IHttpService httpService,
        IConfigurationManager configurationManager,
        IResourcesService resourcesService,
        IRoomsManager roomsManager)
    {
        _httpService = httpService;
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
        _roomsManager = roomsManager;
    }

    public override async Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        if (roomId != "arcade" || parts.Length < 3 || parts[1] != "raw")
        {
            return;
        }

        var rawMessage = parts[2].Trim();
        var webhookUrl = _configurationManager.Configuration.ArcadeWebhookUrl;
        var match = Regex.Match(rawMessage,
            """<div class="broadcast-blue"><b>The "(.*?)" roomevent has started!</b></div>""");
        if (!match.Success)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            Logger.Error("Arcade webhook url is missing.");
            return;
        }

        var eventName = match.Groups[1].Value;
        var room = _roomsManager.GetRoom(roomId);
        var message = _resourcesService.GetString("arcade_event_announce", room.Culture);
        try
        {
            var body = new ArcadeEventWebhookBody
            {
                AvatarUrl = WEBHOOK_AVATAR_URL,
                Username = WEBHOOK_USERNAME,
                Embeds =
                [
                    new ArcadeEventWebhookEmbed
                    {
                        Title = NOTIFICATION_TITLE,
                        Color = NOTIFICATION_COLOR,
                        Description = string.Format(message, eventName)
                    }
                ]
            };
            var response = await _httpService.PostJson<ArcadeEventWebhookBody, object>(webhookUrl, body);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                Logger.Information("Sent arcade announce via webhook successfully.");
            }
            else
            {
                Logger.Error("Error while sending arcade announce via webhook : received = {0}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error while sending arcade announce via webhook");
        }
    }
}
