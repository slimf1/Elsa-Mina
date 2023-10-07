using System.Text.RegularExpressions;
using ElsaMina.Commands.Teams.TeamPreviewOnLink;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using Serilog;

namespace ElsaMina.Commands.Teams.Samples;

public class AddTeam : Command<AddTeam>, INamed
{
    private static readonly Regex TEAM_NAME_FILTER = new("[^\\w\\d\\s+\\-[\\]]");
    private const int MAX_NAME_LENGTH = 70;
    
    public static string Name => "add-team";
    public static IEnumerable<string> Aliases => new[] { "addteam" };

    private readonly ITeamProviderFactory _teamProviderFactory;
    private readonly ITeamRepository _teamRepository;
    private readonly IClockService _clockService;
    private readonly ILogger _logger;

    public AddTeam(ITeamProviderFactory teamProviderFactory,
        ITeamRepository teamRepository,
        IClockService clockService,
        ILogger logger)
    {
        _teamProviderFactory = teamProviderFactory;
        _teamRepository = teamRepository;
        _clockService = clockService;
        _logger = logger;
    }

    public override string HelpMessageKey => "add_team_help_message";

    public override async Task Run(IContext context)
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

        var teamProvider = _teamProviderFactory.GetTeamProvider(link);
        if (teamProvider == null)
        {
            context.ReplyLocalizedMessage("add_team_no_provider");
            return;
        }

        var sharedTeam = await teamProvider.GetTeamExport(link);
        if (sharedTeam?.TeamExport == null)
        {
            context.ReplyLocalizedMessage("add_team_no_export_error");
            return;
        }

        var teamId = name.ToLowerAlphaNum();

        var team = new Team
        {
            Id = teamId,
            Name = name,
            Author = context.Sender.Name,
            Link = link,
            CreationDate = _clockService.CurrentUtcDateTime,
            TeamJson = ShowdownTeams.TeamExportToJson(sharedTeam.TeamExport),
            Format = format,
            Rooms = new List<RoomTeam>
            {
                new()
                {
                    RoomId = context.RoomId,
                    TeamId = teamId
                }
            }
        };

        try
        {
            await _teamRepository.AddAsync(team);
            context.ReplyLocalizedMessage("add_team_success", teamId);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "Could not insert team with id {0}", teamId);
            context.ReplyLocalizedMessage("add_team_failure", exception.Message);
        }
    }
}