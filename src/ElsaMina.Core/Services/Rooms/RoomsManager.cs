using System.Globalization;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.Rooms;

public class RoomsManager : IRoomsManager
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IRoomInfoRepository _roomInfoRepository;
    private readonly IRoomBotParameterValueRepository _roomBotParameterValueRepository;
    private readonly IUserPlayTimeRepository _userPlayTimeRepository;
    private readonly IClockService _clockService;

    private readonly Dictionary<string, IRoom> _rooms = new();
    private readonly TaskQueue _taskQueue = new();

    public RoomsManager(IConfigurationManager configurationManager,
        IParametersFactory parametersFactory,
        IRoomInfoRepository roomInfoRepository,
        IRoomBotParameterValueRepository roomBotParameterValueRepository,
        IUserPlayTimeRepository userPlayTimeRepository,
        IClockService clockService)
    {
        _configurationManager = configurationManager;
        _roomInfoRepository = roomInfoRepository;
        _roomBotParameterValueRepository = roomBotParameterValueRepository;
        _userPlayTimeRepository = userPlayTimeRepository;
        _clockService = clockService;

        RoomParameters = parametersFactory.GetParameters();
    }

    public IReadOnlyDictionary<string, IParameter> RoomParameters { get; }

    public IRoom GetRoom(string roomId)
    {
        return _rooms.TryGetValue(roomId, out var value) ? value : null;
    }

    public bool HasRoom(string roomId)
    {
        return _rooms.ContainsKey(roomId);
    }

    public async Task InitializeRoomAsync(string roomId, IEnumerable<string> lines,
        CancellationToken cancellationToken = default)
    {
        var receivedLines = lines.ToArray();
        var roomTitle = receivedLines
            .FirstOrDefault(line => line.StartsWith("|title|"))?
            .Split("|")[2];
        var users = receivedLines
            .FirstOrDefault(line => line.StartsWith("|users|"))?
            .Split("|")[2]
            .Split(",")[1..];

        Log.Information("Initializing {0}...", roomTitle);
        var roomParameters = await _roomInfoRepository.GetByIdAsync(roomId, cancellationToken);
        if (roomParameters == null)
        {
            Log.Information("Could not find room parameters, inserting in db...");
            roomParameters = new RoomInfo
            {
                Id = roomId
            };
            await _roomInfoRepository.AddAsync(roomParameters, cancellationToken);
            Log.Information("Inserted room parameters for room {0} in db", roomId);
        }

        var localeParameterValue = roomParameters.ParameterValues?
            .FirstOrDefault(parameter => parameter.ParameterId == ParametersConstants.LOCALE);
        var defaultLocale = localeParameterValue?.Value ?? _configurationManager.Configuration.DefaultLocaleCode;
        var room = new Room(roomTitle ?? roomId, roomId, new CultureInfo(defaultLocale))
        {
            Info = roomParameters
        };

        room.AddUsers(users ?? []);

        _rooms[room.RoomId] = room;
        room.InitializeMessageQueueFromLogs(receivedLines);
        Log.Information("Initializing {0} : DONE", roomTitle);
    }

    public void RemoveRoom(string roomId)
    {
        Log.Information("Removing room : {0}", roomId);
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

    public string GetRoomParameter(string roomId, string parameterId)
    {
        var roomParameters = GetRoom(roomId)?.Info?.ParameterValues;
        if (roomParameters == null || !RoomParameters
                .TryGetValue(parameterId, out var parameter))
        {
            return default;
        }

        var parameterValue = roomParameters.FirstOrDefault(v => v.ParameterId == parameterId);
        return parameterValue?.Value ?? parameter.DefaultValue;
    }

    public async Task<bool> SetRoomParameter(string roomId, string parameterId,
        string value)
    {
        var room = GetRoom(roomId);
        var roomParameters = room.Info;
        var parameter = RoomParameters[parameterId];
        var parameterValue = roomParameters.ParameterValues
            .FirstOrDefault(parameterValue => parameterValue.ParameterId == parameterId);
        try
        {
            if (parameterValue == null)
            {
                parameterValue = new RoomBotParameterValue
                {
                    ParameterId = parameterId,
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

            parameter.OnUpdateAction?.Invoke(room, value);
            Log.Information("Saved room parameter: '{0}' = '{1}' for room '{2}'",
                parameterId, value, roomId);
            return true;
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Room parameter save failed: '{0}' = '{1}' for room '{2}'",
                parameterId, value, roomId);
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
            Log.Error(exception, "An error occurred while updating playtime");
        }
    }

    public void Clear()
    {
        _rooms.Clear();
    }

    private async Task UpdateUserPlayTime(IRoom room, string userId, TimeSpan additionalPlayTime)
    {
        Log.Information("Trying to update user playtime : {0} in {1} = +{2}", userId, room.RoomId,
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
            Log.Information("Added user play time for user {0} in {1} : {2}", userId, room.RoomId,
                additionalPlayTime.TotalSeconds);
        }
        else
        {
            savedPlayTime.PlayTime += additionalPlayTime;
            await _userPlayTimeRepository.UpdateAsync(savedPlayTime);
            Log.Information("Updated user play time for user {0} in {1} : +{2}", userId, room.RoomId,
                additionalPlayTime.TotalSeconds);
        }
    }
}