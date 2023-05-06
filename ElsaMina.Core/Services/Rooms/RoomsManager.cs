using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using Serilog;

namespace ElsaMina.Core.Services.Rooms;

public class RoomsManager : IRoomsManager
{
    private readonly ILogger _logger;
    private readonly IConfigurationService _configurationService;
    
    private readonly IDictionary<string, IRoom> _rooms = new Dictionary<string, IRoom>();

    public RoomsManager(ILogger logger,
        IConfigurationService configurationService)
    {
        _logger = logger;
        _configurationService = configurationService;
    }

    public IRoom GetRoom(string roomId)
    {
        return _rooms.ContainsKey(roomId) ? _rooms[roomId] : null;
    }

    public bool HasRoom(string roomId)
    {
        return _rooms.ContainsKey(roomId);
    }

    public void InitializeRoom(string roomId, string roomTitle, IEnumerable<string> userIds)
    {
        _logger.Information($"Initializing {roomTitle}...");
        var room = new Room(roomTitle, roomId, _configurationService.Configuration.DefaultLocaleCode);

        foreach (var userId in userIds)
        {
            room.AddUser(userId);
        }
        
        _rooms[room.RoomId] = room;
        _logger.Information($"Initializing {roomTitle} : DONE");
    }

    public void RemoveRoom(string roomId)
    {
        _logger.Information($"Removing room : {roomId}");
        if (_rooms.ContainsKey(roomId))
        {
            _rooms.Remove(roomId);
        }
    }

    public void AddUserToRoom(string roomId, string userId)
    {
        GetRoom(roomId)?.AddUser(userId);
    }

    public void RemoveUserFromRoom(string roomId, string userId)
    {
        GetRoom(roomId)?.RemoveUser(userId);
    }

    public void RenameUserFromRoom(string roomId, string formerName, string newName)
    {
        GetRoom(roomId)?.RenameUser(formerName, newName);
    }
}