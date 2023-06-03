using System.Globalization;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
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
    private readonly IRepository<RoomParameters, string> _roomParametersRepository;
    private readonly IRoomsManager _roomsManager;
    private readonly IResourcesService _resourcesService;

    public RoomConfig(ILogger logger,
        IRepository<RoomParameters, string> roomParametersRepository,
        IRoomsManager roomsManager,
        IResourcesService resourcesService)
    {
        _logger = logger;
        _roomParametersRepository = roomParametersRepository;
        _roomsManager = roomsManager;
        _resourcesService = resourcesService;
    }

    public async Task Run(IContext context)
    {
        var parts = context.Target.Split(",");
        var roomId = parts[0].Trim().ToLower();
        var locale = parts[1].Trim().ToLower();

        var room = _roomsManager.GetRoom(roomId);
        if (room == null)
        {
            context.ReplyLocalizedMessage("room_config_room_not_found", roomId);
            return;
        }

        if (!_resourcesService.SupportedLocales.Select(culture => culture.Name.ToLower()).Contains(locale))
        {
            context.ReplyLocalizedMessage("room_config_locale_not_found", locale);
            return;
        }

        room.Locale = locale;

        var roomParameters = new RoomParameters
        {
            Id = roomId,
            Locale = locale,
            IsShowingErrorMessages = parts[2] == "on",
            IsCommandAutocorrectEnabled = parts[3] == "on"
        };
        
        try {
            await _roomParametersRepository.UpdateAsync(roomParameters);
            context.Locale = new CultureInfo(locale);
            context.ReplyLocalizedMessage("room_config_success", roomId);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occured while updating room configuration");
            context.ReplyLocalizedMessage("room_config_failure", exception.Message);
        }
    }
}