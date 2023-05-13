using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using Serilog;

namespace ElsaMina.Core.Services.Rooms;

public class RoomsManager : IRoomsManager
{
    private readonly ILogger _logger;
    private readonly IConfigurationManager _configurationManager;
    private readonly IRoomParametersRepository _roomParametersRepository;

    private readonly Dictionary<string, IRoom> _rooms = new();

    public RoomsManager(ILogger logger,
        IConfigurationManager configurationManager,
        IRoomParametersRepository roomParametersRepository)
    {
        _logger = logger;
        _configurationManager = configurationManager;
        _roomParametersRepository = roomParametersRepository;
    }

    public IRoom GetRoom(string roomId)
    {
        return _rooms.ContainsKey(roomId) ? _rooms[roomId] : null;
    }

    public bool HasRoom(string roomId)
    {
        return _rooms.ContainsKey(roomId);
    }

    public async Task InitializeRoom(string roomId, string roomTitle, IEnumerable<string> userIds)
    {
        _logger.Information("Initializing {RoomTitle}...", roomTitle);
        var roomParameters = await _roomParametersRepository.GetByIdAsync(roomId);
        if (roomParameters == null)
        {
            _logger.Information("Could not find room parameters, inserting in db...");
            roomParameters = new RoomParameters
            {
                Id = roomId,
                Locale = _configurationManager.Configuration.DefaultLocaleCode,
                IsShowingErrorMessages = false
            };
            await _roomParametersRepository.AddAsync(roomParameters);
        }
        var defaultLocale = roomParameters.Locale ?? _configurationManager.Configuration.DefaultLocaleCode;
        var room = new Room(roomTitle, roomId, defaultLocale);

        foreach (var userId in userIds)
        {
            room.AddUser(userId);
        }
        
        _rooms[room.RoomId] = room;
        _logger.Information("Initializing {RoomTitle} : DONE", roomTitle);
    }

    public void RemoveRoom(string roomId)
    {
        _logger.Information("Removing room : {RoomId}", roomId);
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

    public void RenameUserInRoom(string roomId, string formerName, string newName)
    {
        GetRoom(roomId)?.RenameUser(formerName, newName);
    }
}