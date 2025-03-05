using System.Globalization;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.Rooms;

public class RoomsManager : IRoomsManager
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IRoomParametersRepository _roomParametersRepository;
    private readonly IRoomBotParameterValueRepository _roomBotParameterValueRepository;
    private readonly IUserPlayTimeRepository _userPlayTimeRepository;
    private readonly IClockService _clockService;

    private readonly Dictionary<string, IRoom> _rooms = new();
    private readonly TaskQueue _taskQueue = new();

    public RoomsManager(IConfigurationManager configurationManager,
        IRoomConfigurationParametersFactory roomConfigurationParametersFactory,
        IRoomParametersRepository roomParametersRepository,
        IRoomBotParameterValueRepository roomBotParameterValueRepository,
        IUserPlayTimeRepository userPlayTimeRepository,
        IClockService clockService)
    {
        _configurationManager = configurationManager;
        _roomParametersRepository = roomParametersRepository;
        _roomBotParameterValueRepository = roomBotParameterValueRepository;
        _userPlayTimeRepository = userPlayTimeRepository;
        _clockService = clockService;

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

    public async Task InitializeRoom(string roomId, IEnumerable<string> lines)
    {
        var receivedLines = lines.ToArray();
        var roomTitle = receivedLines
            .FirstOrDefault(line => line.StartsWith("|title|"))?
            .Split("|")[2];
        var users = receivedLines
            .FirstOrDefault(line => line.StartsWith("|users|"))?
            .Split("|")[2]
            .Split(",")[1..];

        Logger.Information("Initializing {0}...", roomTitle);
        var roomParameters = await _roomParametersRepository.GetByIdAsync(roomId);
        if (roomParameters == null)
        {
            Logger.Information("Could not find room parameters, inserting in db...");
            roomParameters = new RoomParameters
            {
                Id = roomId
            };
            await _roomParametersRepository.AddAsync(roomParameters);
            Logger.Information("Inserted room parameters for room {0} in db", roomId);
        }

        var localeParameterValue = roomParameters.ParameterValues?
            .FirstOrDefault(parameter => parameter.ParameterId == RoomParametersConstants.LOCALE);
        var defaultLocale = localeParameterValue?.Value ?? _configurationManager.Configuration.DefaultLocaleCode;
        var room = new Room(roomTitle ?? roomId, roomId, new CultureInfo(defaultLocale))
        {
            Parameters = roomParameters
        };

        foreach (var userId in users ?? [])
        {
            room.AddUser(userId);
        }

        _rooms[room.RoomId] = room;
        room.InitializeMessageQueueFromLogs(receivedLines);
        Logger.Information("Initializing {0} : DONE", roomTitle);
    }

    public void RemoveRoom(string roomId)
    {
        Logger.Information("Removing room : {0}", roomId);
        _rooms.Remove(roomId);
    }

    public void AddUserToRoom(string roomId, string username)
    {
        GetRoom(roomId)?.AddUser(username);
    }

    public void RemoveUserFromRoom(string roomId, string username)
    {
        var room = GetRoom(roomId);
        if (room == null)
        {
            return;
        }

        var joinDate = room.GetUserJoinDate(username);
        room.RemoveUser(username);
        _taskQueue.Enqueue(async () => await AddPlayTimeForUser(room, username, joinDate));
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
                roomParameters.ParameterValues.Add(parameterValue);
            }
            else
            {
                parameterValue.Value = value;
                await _roomBotParameterValueRepository.UpdateAsync(parameterValue);
            }

            roomBotConfigurationParameter.OnUpdateAction?.Invoke(room, value);
            Logger.Information("Saved room parameter: '{0}' = '{1}' for room '{2}'",
                roomBotParameterId, value, roomId);
            return true;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Room parameter save failed: '{0}' = '{1}' for room '{2}'",
                roomBotParameterId, value, roomId);
            return false;
        }
    }

    public async Task AddPlayTimeForUser(IRoom room, string username, DateTime joinDate)
    {
        var userId = username.ToLowerAlphaNum();
        if (joinDate == DateTime.MinValue)
        {
            return;
        }

        var timeSpan = _clockService.CurrentUtcDateTime - joinDate;
        try
        {
            await UpdateUserPlayTime(room, userId, timeSpan);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "An error occurred while updating playtime");
        }
    }

    public void Clear()
    {
        _rooms.Clear();
    }

    private async Task UpdateUserPlayTime(IRoom room, string userId, TimeSpan additionalPlayTime)
    {
        Logger.Information("Trying to update user playtime : {0} in {1} = +{2}", userId, room.RoomId,
            additionalPlayTime.TotalSeconds);
        var key = Tuple.Create(userId, room.RoomId);
        var savedPlayTime = await _userPlayTimeRepository.GetByIdAsync(key);
        if (savedPlayTime == null)
        {
            await _userPlayTimeRepository.AddAsync(new UserPlayTime
            {
                UserId = userId,
                RoomId = room.RoomId,
                PlayTime = additionalPlayTime
            });
            Logger.Information("Added user play time for user {0} in {1} : {2}", userId, room.RoomId,
                additionalPlayTime.TotalSeconds);
        }
        else
        {
            savedPlayTime.PlayTime += additionalPlayTime;
            await _userPlayTimeRepository.UpdateAsync(savedPlayTime);
            Logger.Information("Updated user play time for user {0} in {1} : +{2}", userId, room.RoomId,
                additionalPlayTime.TotalSeconds);
        }
    }
}