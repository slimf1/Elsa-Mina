using ElsaMina.Core.Client;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.Parsers;
using ElsaMina.Core.Services.PrivateMessages;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Utils;
using Serilog;

namespace ElsaMina.Core.Bot;

public class Bot : IBot
{
    private const long SAME_MESSAGE_COOLDOWN = 3;
    private const int MESSAGE_LENGTH_LIMIT = 125000;

    private readonly ILogger _logger;
    private readonly IClient _client;
    private readonly IConfigurationManager _configurationManager;
    private readonly IClockService _clockService;
    private readonly IContextFactory _contextFactory;
    private readonly ICommandExecutor _commandExecutor;
    private readonly IRoomsManager _roomsManager;
    private readonly IFormatsManager _formatsManager;
    private readonly ILoginService _loginService;
    private readonly IPmSendersManager _pmSendersManager;
    private readonly IParsersManager _parsersManager;
    private readonly IUserDetailsManager _userDetailsManager;

    private readonly SemaphoreSlim _loadRoomSemaphore = new(1, 1);
    private string _currentRoom;
    private string _lastMessage;
    private long _lastMessageTime;
    private bool _disposed;

    public Bot(ILogger logger,
        IClient client,
        IConfigurationManager configurationManager,
        IClockService clockService,
        IContextFactory contextFactory,
        ICommandExecutor commandExecutor,
        IRoomsManager roomsManager,
        IFormatsManager formatsManager,
        ILoginService loginService,
        IPmSendersManager pmSendersManager,
        IParsersManager parsersManager,
        IUserDetailsManager userDetailsManager)
    {
        _logger = logger;
        _client = client;
        _configurationManager = configurationManager;
        _clockService = clockService;
        _contextFactory = contextFactory;
        _commandExecutor = commandExecutor;
        _roomsManager = roomsManager;
        _formatsManager = formatsManager;
        _loginService = loginService;
        _pmSendersManager = pmSendersManager;
        _parsersManager = parsersManager;
        _userDetailsManager = userDetailsManager;
    }

    public async Task Start()
    {
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

        _logger.Information("[Received] ({0}) {1}", room, line);

        if (!_parsersManager.IsInitialized)
        {
            _parsersManager.Initialize();
        }

        await _parsersManager.Parse(parts);

        switch (parts[1])
        {
            case "nametaken":
                _logger.Error("Login failed, check username or password validity. Exiting");
                Environment.Exit(1);
                break;
            case "challstr":
                await Login(string.Join("|", parts[2..]));
                break;
            case "updateuser":
                CheckConnection(parts);
                break;
            case "formats":
                _formatsManager.ParseFormatsFromReceivedLine(line);
                break;
            case "deinit":
                _roomsManager.RemoveRoom(roomId);
                break;
            case "J":
                _roomsManager.AddUserToRoom(roomId, parts[2]);
                break;
            case "L":
                _roomsManager.RemoveUserFromRoom(roomId, parts[2]);
                break;
            case "N":
                _roomsManager.RenameUserInRoom(roomId, parts[3], parts[2]);
                break;
            case "c:":
                await HandleChatMessage(parts[4], parts[3], roomId, long.Parse(parts[2]));
                break;
            case "pm":
                await HandlePrivateMessage(parts[4], parts[2]);
                break;
            case "queryresponse":
                if (parts[2] == "userdetails")
                {
                    _userDetailsManager.HandleReceivedUserDetails(parts[3]);
                }
                break;
        }
    }

    private async Task HandlePrivateMessage(string message, string sender)
    {
        var user = _pmSendersManager.GetUser(sender);
        var (target, command) = ParseMessage(message);
        if (target == null || command == null || !_commandExecutor.HasCommand(command))
        {
            return;
        }

        var context = _contextFactory.GetContext(ContextType.Pm, this, target, user, command);
        try
        {
            await _commandExecutor.TryExecuteCommand(command, context);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "Pm Command execution crashed");
        }
    }

    private async Task HandleChatMessage(string message, string sender, string roomId, long timestamp)
    {
        if (roomId == null || !_roomsManager.HasRoom(roomId))
        {
            return;
        }

        var senderId = sender.ToLowerAlphaNum();
        if (_configurationManager.Configuration.RoomBlacklist.Contains(roomId))
        {
            return;
        }

        var (target, command) = ParseMessage(message);
        if (target == null || command == null)
        {
            return;
        }

        var room = _roomsManager.GetRoom(roomId);
        var context = _contextFactory.GetContext(ContextType.Room, this, target, room.Users[senderId], command,
            room, timestamp);

        try
        {
            await _commandExecutor.TryExecuteCommand(command, context);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "Room Command execution crashed");
        }
    }

    private (string target, string command) ParseMessage(string message)
    {
        var trigger = _configurationManager.Configuration.Trigger;
        var triggerLength = trigger.Length;
        if (message[..triggerLength] != trigger)
        {
            return (null, null);
        }

        var text = message[triggerLength..];
        var spaceIndex = text.IndexOf(" ", StringComparison.Ordinal);
        var command = spaceIndex > 0 ? text[..spaceIndex].ToLower() : text.Trim().ToLower();
        var target = spaceIndex > 0 ? text[(spaceIndex + 1)..] : string.Empty;
        if (string.IsNullOrEmpty(command))
        {
            return (null, null);
        }

        return (target, command);
    }

    private void CheckConnection(string[] parts)
    {
        var name = parts[2][1..];
        if (name.Contains('@'))
        {
            name = name[^2..];
        }
        
        _logger.Information("Connected as : {0}", name);

        foreach (var roomId in _configurationManager.Configuration.Rooms)
        {
            if (_configurationManager.Configuration.RoomBlacklist.Contains(roomId))
            {
                continue;
            }

            _client.Send($"|/join {roomId}");
            Thread.Sleep(250);
        }
    }

    private async Task Login(string challstr)
    {
        _logger.Information("Logging in...");
        var response = await _loginService.Login(challstr);

        if (response?.CurrentUser == null ||
            _configurationManager.Configuration.Name.ToLowerAlphaNum() != response.CurrentUser.UserId)
        {
            _logger.Error("Login failed. Check password validity. Exiting");
            Environment.Exit(1);
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

        _logger.Information("[Sending] {0}", message);

        _client.Send(message);
        _lastMessage = message;
        _lastMessageTime = _clockService.CurrentDateTimeOffset.ToUnixTimeSeconds();
        Thread.Sleep(250);
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