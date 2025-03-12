using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Start;
using ElsaMina.Core.Services.System;

namespace ElsaMina.Core;

public class Bot : IBot
{
    private const int MESSAGE_LENGTH_LIMIT = 125_000;
    private static readonly TimeSpan SAME_MESSAGE_COOLDOWN = TimeSpan.FromSeconds(3);

    private readonly IClient _client;
    private readonly IClockService _clockService;
    private readonly IRoomsManager _roomsManager;
    private readonly IHandlerManager _handlerManager;
    private readonly ISystemService _systemService;
    private readonly IStartManager _startManager;
    private readonly IBotLifecycleManager _lifecycleManager;

    private readonly SemaphoreSlim _initializeRoomSemaphore = new(1, 1);
    private string _currentRoom;
    private string _lastMessage;
    private DateTime _lastMessageTime;
    private bool _disposed;

    public Bot(IClient client,
        IClockService clockService,
        IRoomsManager roomsManager,
        IHandlerManager handlerManager,
        ISystemService systemService,
        IStartManager startManager,
        IBotLifecycleManager lifecycleManager)
    {
        _client = client;
        _clockService = clockService;
        _roomsManager = roomsManager;
        _handlerManager = handlerManager;
        _systemService = systemService;
        _startManager = startManager;
        _lifecycleManager = lifecycleManager;
    }

    public async Task Start()
    {
        _lifecycleManager.OnStart();
        await _startManager.OnStart();
        await _client.Connect();
    }

    public void OnReconnect()
    {
        // On connect / reconnect
        _lifecycleManager.OnReconnect();
    }

    public void OnDisconnect()
    {
        // On disconnect
        _lifecycleManager.OnDisconnect();
        _roomsManager.Clear();
    }

    public async Task HandleReceivedMessage(string message)
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
                await _roomsManager.InitializeRoom(room, lines);
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

        Logger.Information("[Received] ({0}) {1}", room, line);

        if (!_handlerManager.IsInitialized)
        {
            _handlerManager.Initialize();
        }

        await _handlerManager.HandleMessage(parts, roomId);
    }

    public void Send(string message)
    {
        var now = _clockService.CurrentUtcDateTime;
        if ((_lastMessage == message && now - _lastMessageTime < SAME_MESSAGE_COOLDOWN)
            || message.Length > MESSAGE_LENGTH_LIMIT)
        {
            return;
        }

        Logger.Information("[Sending] {0}", message);

        _client.Send(message);
        _lastMessage = message;
        _lastMessageTime = now;
        _systemService.Sleep(TimeSpan.FromMilliseconds(250));
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
            _client.Dispose();
        }

        _disposed = true;
    }
}