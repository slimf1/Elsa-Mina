using System.Text.RegularExpressions;
using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.Samples;

[NamedCommand("add-team", Aliases = ["addteam"])]
public class AddTeam : Command
{
    private const int MAX_NAME_LENGTH = 70;
    private static readonly Regex TEAM_NAME_FILTER = new(@"[^\w\d\s+\-[\]]", RegexOptions.Compiled,
        Constants.REGEX_MATCH_TIMEOUT);

    private readonly ITeamLinkMatchFactory _teamLinkMatchFactory;
    private readonly ITeamRepository _teamRepository;
    private readonly IClockService _clockService;

    public AddTeam(ITeamLinkMatchFactory teamLinkMatchFactory,
        ITeamRepository teamRepository,
        IClockService clockService)
    {
        _teamLinkMatchFactory = teamLinkMatchFactory;
        _teamRepository = teamRepository;
        _clockService = clockService;
    }

    public override string HelpMessageKey => "add_team_help_message";

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        string link;
        string name;
        string format;
        try
        {
            var parts = context.Target.Split(',');
            link = parts[0].Trim();
            name = TEAM_NAME_FILTER.Replace(parts[1].Trim(), string.Empty);
            format = parts[2].Trim();
        }
        catch (Exception)
        {
            ReplyLocalizedHelpMessage(context);
            return;
        }

        if (name.Length > MAX_NAME_LENGTH)
        {
            context.ReplyLocalizedMessage("add_team_name_too_long");
            return;
        }

        var teamLinkMatch = _teamLinkMatchFactory.FindTeamLinkMatch(link);
        if (teamLinkMatch == null)
        {
            context.ReplyLocalizedMessage("add_team_no_provider");
            return;
        }

        var sharedTeam = await teamLinkMatch.GetTeamExport();
        if (sharedTeam?.TeamExport == null)
        {
            context.ReplyLocalizedMessage("add_team_no_export_error");
            return;
        }

        var teamId = name.ToLowerAlphaNum();
        var roomTeams = new List<RoomTeam>
        {
            new()
            {
                RoomId = context.RoomId,
                TeamId = teamId
            }
        };

        // Special case for the French room
        if (context.RoomId is "franais" or "arcade")
        {
            roomTeams.Add(new RoomTeam
            {
                RoomId = context.RoomId == "arcade" ? "franais" : "arcade",
                TeamId = teamId
            });
        }

        var team = new Team
        {
            Id = teamId,
            Name = name,
            Author = context.Sender.Name,
            Link = link,
            CreationDate = _clockService.CurrentUtcDateTime,
            TeamJson = ShowdownTeams.TeamExportToJson(sharedTeam.TeamExport),
            Format = format,
            Rooms = roomTeams
        };

        try
        {
            await _teamRepository.AddAsync(team, cancellationToken);
            context.ReplyLocalizedMessage("add_team_success", teamId);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Could not insert team with id {0}", teamId);
            context.ReplyLocalizedMessage("add_team_failure", exception.Message);
        }
    }
}