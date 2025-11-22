using System.Collections.Concurrent;
using System.Globalization;
using System.Timers;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using ElsaMina.Logging;
using Timer = System.Timers.Timer;

namespace ElsaMina.Core.Services.Rooms;

public class RoomsManager : IRoomsManager, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IRoomInfoRepository _roomInfoRepository;
    private readonly IRoomBotParameterValueRepository _roomBotParameterValueRepository;
    private readonly IUserPlayTimeRepository _userPlayTimeRepository;

    private readonly ConcurrentDictionary<string, IRoom> _rooms = new();
    private readonly SemaphoreSlim _playTimeUpdateSemaphoreSlim = new(1, 1);
    private Timer _processPlayTimeUpdatesTimer;
    private bool _disposed;

    public RoomsManager(IConfiguration configuration,
        IParametersFactory parametersFactory,
        IRoomInfoRepository roomInfoRepository,
        IRoomBotParameterValueRepository roomBotParameterValueRepository,
        IUserPlayTimeRepository userPlayTimeRepository)
    {
        _configuration = configuration;
        _roomInfoRepository = roomInfoRepository;
        _roomBotParameterValueRepository = roomBotParameterValueRepository;
        _userPlayTimeRepository = userPlayTimeRepository;

        RoomParameters = parametersFactory.GetParameters();
    }

    public IReadOnlyDictionary<string, IParameter> RoomParameters { get; }

    public void Initialize()
    {
        StartPlayTimeUpdatesTimer();
    }

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
        var defaultLocale = localeParameterValue?.Value ?? _configuration.DefaultLocaleCode;
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
        _rooms.Remove(roomId, out _);
    }

    public void AddUserToRoom(string roomId, string username)
    {
        GetRoom(roomId)?.AddUser(username);
    }

    public void RemoveUserFromRoom(string roomId, string username)
    {
        GetRoom(roomId)?.RemoveUser(username);
    }

    public async Task ProcessPendingPlayTimeUpdates()
    {
        try
        {
            await _playTimeUpdateSemaphoreSlim.WaitAsync();

            Log.Information("Processing play time updates...");
            foreach (var room in _rooms.Values)
            {
                var playTimeUpdates = room.PendingPlayTimeUpdates.ToList();
                foreach (var (updateUsername, updatePlayTime) in playTimeUpdates)
                {
                    try
                    {
                        await UpdateUserPlayTime(room, updateUsername, updatePlayTime);
                        room.PendingPlayTimeUpdates.Remove(updateUsername);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to update play time for user {0} in room {1}",
                            updateUsername, room.RoomId);
                    }
                }
            }

            await _userPlayTimeRepository.SaveChangesAsync();
        }
        finally
        {
            _playTimeUpdateSemaphoreSlim.Release();
        }
    }

    private async void ProcessPlayTimeUpdatesTimerElapsed(object sender, ElapsedEventArgs e)
    {
        await ProcessPendingPlayTimeUpdates();
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
        if (room == null)
        {
            return false;
        }

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
            }

            await _roomBotParameterValueRepository.SaveChangesAsync();
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

    public void Clear()
    {
        _rooms.Clear();
    }

    private void StartPlayTimeUpdatesTimer()
    {
        _processPlayTimeUpdatesTimer = new Timer(_configuration.PlayTimeUpdatesInterval);
        _processPlayTimeUpdatesTimer.AutoReset = true;
        _processPlayTimeUpdatesTimer.Elapsed += ProcessPlayTimeUpdatesTimerElapsed;
        _processPlayTimeUpdatesTimer.Start();
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
            Log.Information("Added playtime");
        }
        else
        {
            savedPlayTime.PlayTime += additionalPlayTime;
            Log.Information("Updated playtime");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing || _disposed)
        {
            return;
        }

        _processPlayTimeUpdatesTimer?.Stop();
        _processPlayTimeUpdatesTimer?.Dispose();
        _playTimeUpdateSemaphoreSlim?.Dispose();
        _disposed = true;
    }

    ~RoomsManager()
    {
        Dispose(false);
    }
}