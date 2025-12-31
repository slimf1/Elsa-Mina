using System.Collections.Concurrent;
using System.Globalization;
using System.Timers;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;
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
    private readonly IDependencyContainerService _dependencyContainerService;

    private readonly ConcurrentDictionary<string, IRoom> _rooms = new();
    private readonly SemaphoreSlim _playTimeUpdateSemaphoreSlim = new(1, 1);
    private Timer _processPlayTimeUpdatesTimer;
    private bool _disposed;

    public RoomsManager(
        IConfiguration configuration,
        IParametersDefinitionFactory parametersDefinitionFactory,
        IBotDbContextFactory dbContextFactory,
        IRoomUserDataService roomUserDataService,
        IDependencyContainerService dependencyContainerService)
    {
        _configuration = configuration;
        _dbContextFactory = dbContextFactory;
        _roomUserDataService = roomUserDataService;
        _dependencyContainerService = dependencyContainerService;

        ParametersDefinitions = parametersDefinitionFactory.GetParametersDefinitions();
    }

    public IReadOnlyDictionary<Parameter, IParameterDefiniton> ParametersDefinitions { get; }

    public void Initialize() => StartPlayTimeUpdatesTimer();

    public IRoom GetRoom(string roomId) =>
        _rooms.TryGetValue(roomId, out var room) ? room : null;

    public bool HasRoom(string roomId) => _rooms.ContainsKey(roomId);

    // This method is ass and needs refactoring~
    public async Task InitializeRoomAsync(string roomId, IEnumerable<string> lines,
        CancellationToken cancellationToken = default)
    {
        var receivedLines = lines.ToArray();

        // Le titre peut être différent du roomId
        var roomTitle = receivedLines
            .FirstOrDefault(x => x.StartsWith("|title|"))
            ?.Split("|")[2] ?? roomId;

        var users = receivedLines
            .FirstOrDefault(x => x.StartsWith("|users|"))?
            .Split("|")[2]
            .Split(",")[1..];

        Log.Information("Initializing {0}...", roomTitle);

        var dbRoomEntity = await InitializeOrUpdateRoomEntity(roomId, roomTitle, cancellationToken);

        // Construction de la room
        var parameterStore = _dependencyContainerService.Resolve<IRoomParameterStore>();
        parameterStore.InitializeFromRoomEntity(dbRoomEntity);
        var localeCode = await parameterStore.GetValueAsync(Parameter.Locale, cancellationToken);
        var room = new Room(roomTitle,
            roomId,
            new CultureInfo(localeCode ?? _configuration.DefaultLocaleCode),
            parameterStore,
            ParametersDefinitions);

        parameterStore.Room = room;
        room.AddUsers(users ?? []);
        room.InitializeMessageQueueFromLogs(receivedLines);

        _rooms[room.RoomId] = room;

        Log.Information("Initializing {0} : DONE", roomTitle);
    }

    private async Task<SavedRoom> InitializeOrUpdateRoomEntity(string roomId, string roomTitle,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var dbRoom = await dbContext
            .RoomInfo
            .Include(savedRoom => savedRoom.ParameterValues)
            .FirstOrDefaultAsync(savedRoom => savedRoom.Id == roomId, cancellationToken);

        if (dbRoom == null)
        {
            Log.Information("Could not find room parameters, inserting...");

            dbRoom = new SavedRoom { Id = roomId, Title = roomTitle };
            await dbContext.RoomInfo.AddAsync(dbRoom, cancellationToken);
            Log.Information("Inserted room parameters for {0}", roomId);
        }
        else
        {
            dbRoom.Title = roomTitle;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return dbRoom;
    }

    public void RemoveRoom(string roomId)
    {
        Log.Information("Removing room {0}", roomId);
        _rooms.Remove(roomId, out _);
    }

    public void AddUserToRoom(string roomId, string username)
    {
        GetRoom(roomId)?.AddUser(username);
    }

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

    public void RenameUserInRoom(string roomId, string formerName, string newName) =>
        GetRoom(roomId)?.RenameUser(formerName, newName);

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