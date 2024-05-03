﻿using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.Samples;

public class AddTeamToRoom : Command<AddTeamToRoom>, INamed
{
    public static string Name => "add-team-to-room";
    public static List<string> Aliases => ["addteamtoroom", "add-to-room", "add-to-room"];

    private readonly ITeamRepository _teamRepository;

    public AddTeamToRoom(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public override char RequiredRank => '+';

    public override async Task Run(IContext context)
    {
        var teamId = context.Target.ToLowerAlphaNum();
        if (string.IsNullOrEmpty(teamId))
        {
            context.ReplyLocalizedMessage("add_team_to_room_no_arg");
            return;
        }

        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
        {
            context.ReplyLocalizedMessage("add_team_to_room_no_team");
            return;
        }

        if (team.Rooms != null && team.Rooms.Any(roomTeam => roomTeam.RoomId == context.RoomId))
        {
            context.ReplyLocalizedMessage("add_team_to_room_team_already_in_room");
            return;
        }

        team.Rooms ??= new List<RoomTeam>();
        team.Rooms.Add(new RoomTeam
        {
            RoomId = context.RoomId,
            TeamId = team.Id
        });

        try
        {
            await _teamRepository.UpdateAsync(team);
            context.ReplyLocalizedMessage("add_team_to_room_success");
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "An error occurred while adding room to team");
            context.ReplyLocalizedMessage("add_team_to_room_failure", exception.Message);
        }
    }
}