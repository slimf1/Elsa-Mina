using System.Globalization;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.Rooms;

public class RoomsManager : IRoomsManager
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IRoomParametersRepository _roomParametersRepository;
    private readonly IRoomBotParameterValueRepository _roomBotParameterValueRepository;

    private readonly Dictionary<string, IRoom> _rooms = new();

    public RoomsManager(IConfigurationManager configurationManager,
        IRoomConfigurationParametersFactory roomConfigurationParametersFactory,
        IRoomParametersRepository roomParametersRepository,
        IRoomBotParameterValueRepository roomBotParameterValueRepository)
    {
        _configurationManager = configurationManager;
        _roomParametersRepository = roomParametersRepository;
        _roomBotParameterValueRepository = roomBotParameterValueRepository;

        RoomBotConfigurationParameters = roomConfigurationParametersFactory.GetParameters();
    }

    public IReadOnlyDictionary<string, IRoomBotConfigurationParameter> RoomBotConfigurationParameters { get; }

    public IRoom GetRoom(string roomId)
    {
        return _rooms.TryGetValue(roomId, out var value) ? value : null;
    }

    public bool HasRoom(string roomId)
    {
        return _rooms.ContainsKey(roomId);
    }

    public async Task InitializeRoom(string roomId, string roomTitle, IEnumerable<string> userIds)
    {
        Logger.Current.Information("Initializing {0}...", roomTitle);
        var roomParameters = await _roomParametersRepository.GetByIdAsync(roomId);
        if (roomParameters == null)
        {
            Logger.Current.Information("Could not find room parameters, inserting in db...");
            roomParameters = new RoomParameters
            {
                Id = roomId,
            };
            await _roomParametersRepository.AddAsync(roomParameters);
            Logger.Current.Information("Inserted room parameters for room {0} in db", roomId);
        }
        
        var localeParameterValue = roomParameters.ParameterValues?
            .FirstOrDefault(parameter => parameter.ParameterId == RoomParametersConstants.LOCALE);
        var defaultLocale = localeParameterValue?.Value ?? _configurationManager.Configuration.DefaultLocaleCode;
        var room = new Room(roomTitle, roomId, new CultureInfo(defaultLocale))
        {
            Parameters = roomParameters
        };

        foreach (var userId in userIds)
        {
            room.AddUser(userId);
        }

        _rooms[room.RoomId] = room;
        Logger.Current.Information("Initializing {0} : DONE", roomTitle);
    }

    public void RemoveRoom(string roomId)
    {
        Logger.Current.Information("Removing room : {0}", roomId);
        _rooms.Remove(roomId);
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

    public string GetRoomBotConfigurationParameterValue(string roomId, string roomBotParameterId)
    {
        var roomParameters = GetRoom(roomId)?.Parameters?.ParameterValues;
        if (roomParameters == null || !RoomBotConfigurationParameters
            .TryGetValue(roomBotParameterId, out var roomBotConfigurationParameter))
        {
            return default;
        }

        var roomBotParameterValue = roomParameters.FirstOrDefault(v => v.ParameterId == roomBotParameterId);
        return roomBotParameterValue?.Value ?? roomBotConfigurationParameter.DefaultValue;
    }

    public async Task<bool> SetRoomBotConfigurationParameterValue(string roomId, string roomBotParameterId,
        string value)
    {
        var room = GetRoom(roomId);
        var roomParameters = room.Parameters;
        var roomBotConfigurationParameter = RoomBotConfigurationParameters[roomBotParameterId];
        var parameterValue = roomParameters.ParameterValues
            .FirstOrDefault(parameterValue => parameterValue.ParameterId == roomBotParameterId);
        try
        {
            if (parameterValue == null)
            {
                parameterValue = new RoomBotParameterValue
                {
                    ParameterId = roomBotParameterId,
                    RoomId = roomId,
                    Value = value
                };

                await _roomBotParameterValueRepository.AddAsync(parameterValue);
            }
            else
            {
                parameterValue.Value = value;
                await _roomBotParameterValueRepository.UpdateAsync(parameterValue);
            }

            roomBotConfigurationParameter.OnUpdateAction?.Invoke(room, value);
            Logger.Current.Information("Saved room parameter: '{0}' = '{1}' for room '{2}'",
                roomBotParameterId, value, roomId);
            return true;
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "Room parameter save failed: '{0}' = '{1}' for room '{2}'",
                roomBotParameterId, value, roomId);
            return false;
        }
    }
}