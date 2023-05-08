using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using Serilog;

namespace ElsaMina.Commands.RoomDashboard;

public class RoomConfig : ICommand
{
    public static string Name => "room-config";
    public static IEnumerable<string> Aliases => new[] { "roomconfig", "rc" };
    public bool IsWhitelistOnly => true; // todo : only authed used from room
    public bool IsPrivateMessageOnly => true;

    private readonly ILogger _logger;
    private readonly IRoomParametersRepository _roomParametersRepository;

    public RoomConfig(ILogger logger, IRoomParametersRepository roomParametersRepository)
    {
        _logger = logger;
        _roomParametersRepository = roomParametersRepository;
    }

    public async Task Run(IContext context)
    {
        try
        {
            var parts = context.Target.Split(",");
            var roomId = parts[0].Trim().ToLower();
            var roomParameters = new RoomParameters
            {
                Id = roomId,
                Locale = parts[1].Trim().ToLower(),
                IsShowingErrorMessages = parts[2] == "on",
                IsCommandAutocorrectEnabled = parts[3] == "on"
            };
            await _roomParametersRepository.UpdateAsync(roomParameters);
            context.Reply(context.GetString("room_config_success", roomId));
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occured while updating room configuration");
            context.Reply(context.GetString("room_config_failure", exception.Message));
        }
    }
}