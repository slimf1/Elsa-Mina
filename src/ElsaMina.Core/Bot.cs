using ElsaMina.Core.Parsers;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.System;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core;

public class Bot : IBot
{
    private const long SAME_MESSAGE_COOLDOWN = 3;
    private const int MESSAGE_LENGTH_LIMIT = 125000;

    private readonly IClient _client;
    private readonly IConfigurationManager _configurationManager;
    private readonly IClockService _clockService;
    private readonly IRoomsManager _roomsManager;
    private readonly IFormatsManager _formatsManager;
    private readonly ILoginService _loginService;
    private readonly IParsersManager _parsersManager;
    private readonly ISystemService _systemService;
    private readonly ITemplatesManager _templatesManager;
    private readonly ICommandExecutor _commandExecutor;

    private readonly SemaphoreSlim _loadRoomSemaphore = new(1, 1);
    private string _currentRoom;
    private string _lastMessage;
    private long _lastMessageTime;
    private bool _disposed;

    public Bot(IClient client,
        IConfigurationManager configurationManager,
        IClockService clockService,
        IRoomsManager roomsManager,
        IFormatsManager formatsManager,
        ILoginService loginService,
        IParsersManager parsersManager,
        ISystemService systemService,
        ITemplatesManager templatesManager,
        ICommandExecutor commandExecutor)
    {
        _client = client;
        _configurationManager = configurationManager;
        _clockService = clockService;
        _roomsManager = roomsManager;
        _formatsManager = formatsManager;
        _loginService = loginService;
        _parsersManager = parsersManager;
        _systemService = systemService;
        _templatesManager = templatesManager;
        _commandExecutor = commandExecutor;
    }

    public async Task Start()
    {
        await _templatesManager.PreCompileTemplates();
        await _commandExecutor.OnBotStartUp();
        await _client.Connect();
    }

    public async Task HandleReceivedMessage(string message)
    {
        var lines = message.Split("\n");
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
                await _loadRoomSemaphore.WaitAsync();
                await LoadRoom(room, message);
            }
            finally
            {
                _loadRoomSemaphore.Release();
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

        Logger.Current.Information("[Received] ({0}) {1}", room, line);

        if (!_parsersManager.IsInitialized)
        {
            await _parsersManager.Initialize();
        }

        await _parsersManager.Parse(parts, roomId);

        switch (parts[1])
        {
            case "challstr":
                await Login(string.Join("|", parts[2..]));
                break;
            case "updateuser":
                CheckConnection(parts);
                break;
            case "formats":
                _formatsManager.ParseFormatsFromReceivedLine(line);
                break;
        }
    }

    private void CheckConnection(string[] parts)
    {
        var name = parts[2][1..];
        if (name.Contains('@'))
        {
            name = name[^2..];
        }
        
        Logger.Current.Information("Connected as : {0}", name);

        foreach (var roomId in _configurationManager.Configuration.Rooms)
        {
            if (_configurationManager.Configuration.RoomBlacklist.Contains(roomId))
            {
                continue;
            }

            _client.Send($"|/join {roomId}");
            _systemService.Sleep(250);
        }
    }

    private async Task Login(string challstr)
    {
        Logger.Current.Information("Logging in...");
        var response = await _loginService.Login(challstr);

        if (response?.CurrentUser == null ||
            _configurationManager.Configuration.Name.ToLowerAlphaNum() != response.CurrentUser.UserId)
        {
            Logger.Current.Error("Login failed. Check password validity. Exiting");
            _systemService.Kill();
            return;
        }
        
        _client.Send($"|/trn {response.CurrentUser.Username},0,{response.Assertion}");
    }

    private async Task LoadRoom(string roomId, string message)
    {
        var parts = message.Split("\n");
        var roomTitle = parts[2].Split("|")[2];
        var users = parts[3].Split("|")[2].Split(",")[1..];

        await _roomsManager.InitializeRoom(roomId, roomTitle, users);
    }

    public void Send(string message)
    {
        if ((_lastMessage == message &&
             _clockService.CurrentDateTimeOffset.ToUnixTimeSeconds() - _lastMessageTime < SAME_MESSAGE_COOLDOWN) ||
            message.Length > MESSAGE_LENGTH_LIMIT)
        {
            return;
        }

        Logger.Current.Information("[Sending] {0}", message);

        _client.Send(message);
        _lastMessage = message;
        _lastMessageTime = _clockService.CurrentDateTimeOffset.ToUnixTimeSeconds();
        _systemService.Sleep(250);
    }

    public void Say(string roomId, string message)
    {
        Send($"{roomId}|{message}");
    }

    public override string ToString()
    {
        return $"{nameof(Bot)}[{nameof(_currentRoom)}: {_currentRoom}, " +
               $"{nameof(_lastMessage)}: {_lastMessage}," +
               $"{nameof(_lastMessageTime)}: {_lastMessageTime}]";
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