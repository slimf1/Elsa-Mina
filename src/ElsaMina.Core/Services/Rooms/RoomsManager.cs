using System.Collections.Concurrent;
using System.Globalization;
using System.Timers;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using ElsaMina.Logging;
using Microsoft.EntityFrameworkCore;
using Timer = System.Timers.Timer;

namespace ElsaMina.Core.Services.Rooms;

public class RoomsManager : IRoomsManager, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IBotDbContextFactory _dbContextFactory;
    private readonly IRoomUserDataService _roomUserDataService;

    private readonly ConcurrentDictionary<string, IRoom> _rooms = new();
    private readonly SemaphoreSlim _playTimeUpdateSemaphoreSlim = new(1, 1);
    private Timer _processPlayTimeUpdatesTimer;
    private bool _disposed;

    public RoomsManager(
        IConfiguration configuration,
        IParametersFactory parametersFactory,
        IBotDbContextFactory dbContextFactory,
        IRoomUserDataService roomUserDataService)
    {
        _configuration = configuration;
        _dbContextFactory = dbContextFactory;
        _roomUserDataService = roomUserDataService;

        RoomParameters = parametersFactory.GetParameters();
    }

    public IReadOnlyDictionary<string, IParameter> RoomParameters { get; }

    public void Initialize() => StartPlayTimeUpdatesTimer();

    public IRoom GetRoom(string roomId) =>
        _rooms.TryGetValue(roomId, out var room) ? room : null;

    public bool HasRoom(string roomId) => _rooms.ContainsKey(roomId);

    public async Task InitializeRoomAsync(string roomId, IEnumerable<string> lines, CancellationToken cancellationToken = default)
    {
        var receivedLines = lines.ToArray();
        var roomTitle = receivedLines
            .FirstOrDefault(x => x.StartsWith("|title|"))
            ?.Split("|")[2];

        var users = receivedLines
            .FirstOrDefault(x => x.StartsWith("|users|"))
            ?.Split("|")[2]
            ?.Split(",")[1..];

        Log.Information("Initializing {0}...", roomTitle);

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var roomParameters = await dbContext.RoomInfo
            .Include(r => r.ParameterValues)
            .FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);

        if (roomParameters == null)
        {
            Log.Information("Could not find room parameters, inserting...");

            roomParameters = new DataAccess.Models.Room { Id = roomId };
            await dbContext.RoomInfo.AddAsync(roomParameters, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            Log.Information("Inserted room parameters for {0}", roomId);
        }

        var localeValue = roomParameters.ParameterValues?
            .FirstOrDefault(p => p.ParameterId == ParametersConstants.LOCALE)
            ?.Value;

        var locale = localeValue ?? _configuration.DefaultLocaleCode;

        var room = new Room(roomTitle ?? roomId, roomId, new CultureInfo(locale))
        {
            Info = roomParameters
        };

        room.AddUsers(users ?? Array.Empty<string>());

        _rooms[room.RoomId] = room;

        room.InitializeMessageQueueFromLogs(receivedLines);

        Log.Information("Initializing {0} : DONE", roomTitle);
    }

    public void RemoveRoom(string roomId)
    {
        Log.Information("Removing room {0}", roomId);
        _rooms.Remove(roomId, out _);
    }

    public void AddUserToRoom(string roomId, string username) =>
        GetRoom(roomId)?.AddUser(username);

    public void RemoveUserFromRoom(string roomId, string username) =>
        GetRoom(roomId)?.RemoveUser(username);

    public async Task ProcessPendingPlayTimeUpdates()
    {
        try
        {
            await _playTimeUpdateSemaphoreSlim.WaitAsync();

            Log.Information("Processing play time updates...");

            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            foreach (var room in _rooms.Values)
            {
                var roomId = room.RoomId;
                var updates = room.PendingPlayTimeUpdates.ToList();

                foreach (var (username, playTime) in updates)
                {
                    try
                    {
                        await _roomUserDataService.IncrementUserPlayTime(roomId, username, playTime);
                        room.PendingPlayTimeUpdates.Remove(username);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to update play time for {0} in room {1}",
                            username, room.RoomId);
                    }
                }
            }

            await dbContext.SaveChangesAsync();
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

    public void RenameUserInRoom(string roomId, string oldName, string newName) =>
        GetRoom(roomId)?.RenameUser(oldName, newName);

    public string GetRoomParameter(string roomId, string parameterId)
    {
        var room = GetRoom(roomId);
        if (room?.Info?.ParameterValues == null) return default;

        if (!RoomParameters.TryGetValue(parameterId, out var parameter)) 
            return default;

        var value = room.Info.ParameterValues
            .FirstOrDefault(v => v.ParameterId == parameterId)
            ?.Value;

        return value ?? parameter.DefaultValue;
    }

    public async Task<bool> SetRoomParameter(string roomId, string parameterId, string value)
    {
        var room = GetRoom(roomId);
        if (room == null)
        {
            return false;
        }

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var roomInfo = room.Info;
        var parameter = RoomParameters[parameterId];

        var existingValue = roomInfo.ParameterValues
            .FirstOrDefault(x => x.ParameterId == parameterId);

        try
        {
            if (existingValue == null)
            {
                var newValue = new RoomBotParameterValue
                {
                    ParameterId = parameterId,
                    RoomId = roomId,
                    Value = value
                };

                await dbContext.RoomBotParameterValues.AddAsync(newValue);
                roomInfo.ParameterValues.Add(newValue);
            }
            else
            {
                existingValue.Value = value;
                dbContext.RoomBotParameterValues.Update(existingValue);
            }

            await dbContext.SaveChangesAsync();

            parameter.OnUpdateAction?.Invoke(room, value);

            Log.Information("Saved room parameter: '{0}' = '{1}' for '{2}'",
                parameterId, value, roomId);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save parameter '{0}' = '{1}' for room '{2}'",
                parameterId, value, roomId);
            return false;
        }
    }

    public void Clear() => _rooms.Clear();

    private void StartPlayTimeUpdatesTimer()
    {
        _processPlayTimeUpdatesTimer = new Timer(_configuration.PlayTimeUpdatesInterval)
        {
            AutoReset = true
        };

        _processPlayTimeUpdatesTimer.Elapsed += ProcessPlayTimeUpdatesTimerElapsed;
        _processPlayTimeUpdatesTimer.Start();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing || _disposed) return;

        _processPlayTimeUpdatesTimer?.Stop();
        _processPlayTimeUpdatesTimer?.Dispose();
        _playTimeUpdateSemaphoreSlim?.Dispose();
        _disposed = true;
    }

    ~RoomsManager() => Dispose(false);
}
