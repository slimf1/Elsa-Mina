using ElsaMina.Core.Client;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ElsaMina.Core.Bot;

public class Bot : IBot
{
    private const string LOGIN_URL = "http://play.pokemonshowdown.com/action.php";
    private const long SAME_MESSAGE_COOLDOWN = 3;
    private const int MESSAGE_LENGTH_LIMIT = 125000;

    private readonly ILogger _logger;
    private readonly IClient _client;
    private readonly IConfigurationService _configurationService;
    private readonly IHttpService _httpService;
    private readonly IClockService _clockService;
    private readonly IContextFactory _contextFactory;
    private readonly ICommandExecutor _commandExecutor;

    private readonly List<string> _formats = new();
    private string? _currentRoom;
    private string? _lastMessage;
    private long? _lastMessageTime;
    private bool _disposed;

    public Bot(ILogger logger,
        IClient client,
        IConfigurationService configurationService,
        IHttpService httpService,
        IClockService clockService,
        IContextFactory contextFactory,
        ICommandExecutor commandExecutor)
    {
        _logger = logger;
        _client = client;
        _configurationService = configurationService;
        _httpService = httpService;
        _clockService = clockService;
        _contextFactory = contextFactory;
        _commandExecutor = commandExecutor;
    }

    public IDictionary<string, IRoom> Rooms { get; } = new Dictionary<string, IRoom>();

    public IEnumerable<string> Formats => _formats;

    public async Task Start()
    {
        await _client.Connect();
    }

    public async Task HandleReceivedMessage(string message)
    {
        var lines = message.Split("\n");
        string? room = null;
        if (lines[0].Length > 0 && lines[0][0] == '>')
        {
            room = lines[0][1..];
            _currentRoom = room;
        }

        if (lines.Length > 2 && lines[1].StartsWith("|init|chat"))
        {
            LoadRoom(room, message);
        }
        else
        {
            foreach (var line in lines)
            {
                await ReadLine(room, line);
            }
        }
    }

    private async Task ReadLine(string? room, string line)
    {
        var parts = line.Split("|");
        if (parts.Length < 2)
        {
            return;
        }

        var roomId = room ?? _currentRoom;

        _logger.Information($"[Received] ({room}) {line}");

        switch (parts[1])
        {
            case "nametaken":
                throw new Exception("Connection failed");
            case "challstr":
                await Login(string.Join("|", parts[2..]));
                break;
            case "updateuser":
                CheckConnection(parts);
                break;
            case "formats":
                ParseFormats(line);
                break;
            case "deinit":
                if (roomId != null && Rooms.ContainsKey(roomId))
                {
                    Rooms.Remove(roomId);
                }
                break;
            case "J":
                if (roomId != null && Rooms.ContainsKey(roomId))
                {
                    Rooms[roomId].AddUser(parts[2]);
                }
                break;
            case "L":
                if (roomId != null && Rooms.ContainsKey(roomId))
                {
                    Rooms[roomId].RemoveUser(parts[2]);
                }
                break;
            case "N":
                if (roomId != null && Rooms.ContainsKey(roomId))
                {
                    Rooms[roomId].RenameUser(parts[3], parts[2]);
                }
                break;
            case "c:":
                await HandleChatMessage(parts[4], parts[3], roomId, long.Parse(parts[2]));
                break;
        }
    }

    private async Task HandleChatMessage(string message, string sender, string? roomId, long timestamp)
    {
        if (roomId == null || !Rooms.ContainsKey(roomId))
        {
            return;
        }
        
        var senderId = sender.ToLowerAlphaNum();
        if (_configurationService.Configuration?.RoomBlacklist?.Contains(roomId) == true)
        {
            return;
        }

        if (!Rooms.ContainsKey(roomId))
        {
            return;
        }
        
        var (target, command) = ParseMessage(message);
        if (target == null || command == null || !_commandExecutor.HasCommand(command))
        {
            return;
        }
        
        var room = Rooms[roomId];
        var context = _contextFactory.GetContext(ContextType.Room, this, target, room.Users[senderId], command,
            room, timestamp);

        try
        {
            await _commandExecutor.TryExecuteCommand(command, context);
        } 
        catch (Exception exception)
        {
            _logger.Error(exception, "Command execution crashed");
        }
        
    }

    private (string? target, string? command) ParseMessage(string message)
    {
        var trigger = _configurationService.Configuration?.Trigger ?? "-";
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

    private void ParseFormats(string line)
    {
        var formats = line.Split("|")[5..];
        foreach (var format in formats)
        {
            if (!format.StartsWith("[Gen"))
            {
                continue;
            }
            _formats.Add(format.Split(",")[0]);
        }
        
    }

    private void CheckConnection(string[] parts)
    {
        var name = parts[2][1..];
        if (name.Contains('@'))
        {
            name = name[^2..];
        }

        if (name == _configurationService.Configuration?.Name)
        {
            _logger.Information($"Connection successful, logged in as {name}");

            foreach (var roomId in _configurationService.Configuration?.Rooms ?? Enumerable.Empty<string>())
            {
                if (_configurationService.Configuration?.RoomBlacklist?.Contains(roomId) ?? false)
                {
                    continue;
                }

                _client.Send($"|/join {roomId}");
                Thread.Sleep(250);
            }
        }
    }

    private async Task Login(string challstr)
    {
        _logger.Information("Logging in...");
        var parameters = new Dictionary<string, string> // Cas d'erreur, deserialisation auto, retry après qq secondes
        {
            ["act"] = "login",
            ["name"] = _configurationService.Configuration?.Name ?? string.Empty,
            ["pass"] = _configurationService.Configuration?.Password ?? string.Empty,
            ["challstr"] = challstr
        };
        var textResponse = await _httpService.PostFormAsync(LOGIN_URL, parameters);
        var jsonResponse = (JObject)JsonConvert.DeserializeObject(textResponse[1..]);
        var nonce = jsonResponse?.GetValue("assertion");
        _client.Send($"|/trn {_configurationService.Configuration?.Name},0,{nonce}");
    }

    private void LoadRoom(string? roomId, string message)
    {
        var parts = message.Split("\n");
        var roomTitle = parts[2].Split("|")[2];
        var users = parts[3].Split("|")[2].Split(",")[1..];

        _logger.Information($"Initializing {roomTitle}...");

        var room = new Room(roomTitle, roomId, "fr-FR"); // TODO: factory ?/ locale
        foreach (var user in users)
        {
            room.AddUser(user);
        }

        Rooms[room.RoomId] = room;

        _logger.Information($"Initializing {roomTitle} : DONE");
    }

    public void Send(string message)
    {
        if ((_lastMessage == message &&
             _clockService.Now.ToUnixTimeSeconds() - _lastMessageTime < SAME_MESSAGE_COOLDOWN) ||
            message.Length > MESSAGE_LENGTH_LIMIT)
        {
            return;
        }

        _logger.Information($"[Sending] {message}");

        _client.Send(message);
        _lastMessage = message;
        _lastMessageTime = _clockService.Now.ToUnixTimeSeconds();
        Thread.Sleep(250);
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