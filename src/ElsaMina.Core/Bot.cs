using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Start;
using ElsaMina.Core.Services.System;
using ElsaMina.Logging;

namespace ElsaMina.Core;

public class Bot : IBot
{
    private const int MESSAGE_LENGTH_LIMIT = 125_000;
    private static readonly TimeSpan SAME_MESSAGE_COOLDOWN = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan SEND_MESSAGE_COOLDOWN = TimeSpan.FromMilliseconds(250);

    private readonly IClient _client;
    private readonly IClockService _clockService;
    private readonly IRoomsManager _roomsManager;
    private readonly IHandlerManager _handlerManager;
    private readonly ISystemService _systemService;
    private readonly IStartManager _startManager;

    private readonly SemaphoreSlim _initializeRoomSemaphore = new(1, 1);
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private string _currentRoom;
    private string _lastMessage;
    private DateTime _lastMessageTime;
    private bool _disposed;

    public Bot(IClient client,
        IClockService clockService,
        IRoomsManager roomsManager,
        IHandlerManager handlerManager,
        ISystemService systemService,
        IStartManager startManager)
    {
        _client = client;
        _clockService = clockService;
        _roomsManager = roomsManager;
        _handlerManager = handlerManager;
        _systemService = systemService;
        _startManager = startManager;
    }

    public async Task StartAsync()
    {
        await _startManager.LoadStaticDataAsync(_cancellationTokenSource.Token);
        await _client.Connect();
    }

    public void OnReconnect()
    {
        // Empty
    }

    public void OnDisconnect()
    {
        _roomsManager.Clear();
    }
    
    public void OnExit()
    {
        Log.Information("Exiting bot...");
        _roomsManager.ProcessPendingPlayTimeUpdates();
    }

    public async Task HandleReceivedMessageAsync(string message)
    {
        var lines = message.Split("\n").Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        if (lines.Length == 0)
        {
            return;
        }

        string room = null;
        if (lines[0].Length > 0 && lines[0][0] == '>')
        {
            room = lines[0][1..];
            _currentRoom = room;
        }

        if (lines.Length > 2 && lines[1].StartsWith("|init|chat"))
        {
            try
            {
                await _initializeRoomSemaphore.WaitAsync();
                await _roomsManager.InitializeRoomAsync(room, lines, _cancellationTokenSource.Token);
            }
            finally
            {
                _initializeRoomSemaphore.Release();
            }
        }
        else
        {
            foreach (var line in lines)
            {
                await ReadLine(room, line);
            }
        }
    }

    private async Task ReadLine(string room, string line)
    {
        var parts = line.Split("|");
        if (parts.Length < 2)
        {
            return;
        }

        var roomId = room ?? _currentRoom;

        Log.Debug("[Received] ({0}) {1}", room, line);

        if (!_handlerManager.IsInitialized)
        {
            _handlerManager.Initialize();
        }
        
        await _handlerManager.HandleMessageAsync(parts, roomId, _cancellationTokenSource.Token);
    }

    public void Send(string message)
    {
        var now = _clockService.CurrentUtcDateTime;
        if ((_lastMessage == message && now - _lastMessageTime < SAME_MESSAGE_COOLDOWN)
            || message.Length > MESSAGE_LENGTH_LIMIT)
        {
            return;
        }

        Log.Debug("[Sending] {0}", message);

        _client.Send(message);
        _lastMessage = message;
        _lastMessageTime = now;
        _systemService.Sleep(SEND_MESSAGE_COOLDOWN);
    }

    public void Say(string roomId, string message)
    {
        Send($"{roomId}|{message}");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _client.Dispose();
        }

        _disposed = true;
    }
}