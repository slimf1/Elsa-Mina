using System.Text.RegularExpressions;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Replays;

public class ReplaysHandler : ChatMessageHandler
{
    private static readonly Regex REPLAY_URL_REGEX =
        new(@"https:\/\/(replay\.pokemonshowdown\.com\/(\w{1,30}-){0,1}\w{2,30}-\d{1,30}(-\w{33}){0,1})",
            RegexOptions.Compiled, Constants.REGEX_MATCH_TIMEOUT);

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

    public override async Task HandleMessageAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var isReplayPreviewEnabled = _roomsManager.GetRoomParameter(
            context.RoomId, ParametersConstants.IS_SHOWING_REPLAYS_PREVIEW).ToBoolean();
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
            Log.Information("Fetching replay info from : {0}", replayLink);
            var response = await _httpService.GetAsync<ReplayDto>(replayLink, cancellationToken: cancellationToken);
            var replayInfo = response.Data;
            var teams = ReplaysHelper.GetTeamsFromLog(replayInfo.Log);
            var template = await _templatesManager.GetTemplateAsync("Replays/ReplayPreview", new ReplayPreviewViewModel
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
                Views = replayInfo.Views,
                Sender = context.Sender.Name
            });

            context.ReplyHtml(template.RemoveNewlines());
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Failed to get replay info");
        }
    }
}