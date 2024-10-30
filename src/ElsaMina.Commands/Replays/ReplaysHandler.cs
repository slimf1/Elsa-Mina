using System.Text.RegularExpressions;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Replays;

// Todo : gérer les battles >2 btl
public class ReplaysHandler : ChatMessageHandler
{
    private const string PLAYER1_IDENTIFIER = "p1";
    private const string PLAYER2_IDENTIFIER = "p2";
    private const string PLAYER3_IDENTIFIER = "p3";
    private const string PLAYER4_IDENTIFIER = "p4";

    private static readonly Regex REPLAY_URL_REGEX =
        new(@"https:\/\/(replay\.pokemonshowdown\.com\/\w{2,30}-\d{1,30}(-\w{33}){0,1})");

    private readonly IHttpService _httpService;
    private readonly ITemplatesManager _templatesManager;

    public ReplaysHandler(IContextFactory contextFactory,
        IHttpService httpService,
        ITemplatesManager templatesManager) : base(contextFactory)
    {
        _httpService = httpService;
        _templatesManager = templatesManager;
    }

    public override string Identifier => nameof(ChatMessageHandler);

    protected override async Task HandleMessage(IContext context)
    {
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
            var has4Players = replayInfo.Players.Count == 4;
            var template = await _templatesManager.GetTemplate("Replays/ReplayPreview", new ReplayPreviewViewModel
            {
                Culture = context.Culture,
                Format = replayInfo.Format,
                Rating = replayInfo.Rating,
                Player1 = replayInfo.Players[0],
                Player2 = replayInfo.Players[1],
                Has4Players = has4Players,
                Player3 = has4Players ? replayInfo.Players[2] : null,
                Player4 = has4Players ? replayInfo.Players[3] : null,
                Player1Species = teams[PLAYER1_IDENTIFIER],
                Player2Species = teams[PLAYER2_IDENTIFIER],
                Player3Species = has4Players ? teams[PLAYER3_IDENTIFIER] : [],
                Player4Species = has4Players ? teams[PLAYER4_IDENTIFIER] : [],
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