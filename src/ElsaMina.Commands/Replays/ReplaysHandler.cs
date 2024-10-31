using System.Text.RegularExpressions;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Replays;

public class ReplaysHandler : ChatMessageHandler
{
    private static readonly Regex REPLAY_URL_REGEX =
        new(@"https:\/\/(replay\.pokemonshowdown\.com\/(\w{1,30}-){0,1}\w{2,30}-\d{1,30}(-\w{33}){0,1})");

    private readonly IHttpService _httpService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IRoomsManager _roomsManager;

    public ReplaysHandler(IContextFactory contextFactory,
        IHttpService httpService,
        ITemplatesManager templatesManager,
        IRoomsManager roomsManager) : base(contextFactory)
    {
        _httpService = httpService;
        _templatesManager = templatesManager;
        _roomsManager = roomsManager;
    }

    public override string Identifier => nameof(ChatMessageHandler);

    protected override async Task HandleMessage(IContext context)
    {
        var isReplayPreviewEnabled = _roomsManager.GetRoomBotConfigurationParameterValue(
            context.RoomId, RoomParametersConstants.IS_SHOWING_REPLAYS_PREVIEW).ToBoolean();
        if (!isReplayPreviewEnabled)
        {
            return;
        }

        var match = REPLAY_URL_REGEX.Match(context.Message);
        if (!match.Success)
        {
            return;
        }

        var replayLink = match.Value.Trim();
        if (replayLink.EndsWith('/'))
        {
            replayLink = replayLink[..^1];
        }
        replayLink += ".json";

        // risqué ?
        try
        {
            Logger.Information("Fetching replay info from : {0}", replayLink);
            var replayInfo = await _httpService.Get<ReplayDto>(replayLink);
            var teams = ReplaysHelper.GetTeamsFromLog(replayInfo.Log);
            var template = await _templatesManager.GetTemplate("Replays/ReplayPreview", new ReplayPreviewViewModel
            {
                Culture = context.Culture,
                Format = replayInfo.Format,
                Rating = replayInfo.Rating,
                Players = teams.Select((team, index) => new ReplayPlayer
                {
                    Name = replayInfo.Players[index],
                    Team = team.Value
                }).ToList(),
                Date = Time.GetDateTimeFromUnixTime(replayInfo.UploadTime),
                Views = replayInfo.Views
            });

            context.SendHtml(template.RemoveNewlines());
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Failed to get replay info");
        }
    }
}