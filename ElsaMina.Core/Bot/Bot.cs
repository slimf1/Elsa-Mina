using ElsaMina.Core.Client;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElsaMina.Core.Bot;

public class Bot : IBot
{
    private const string LOGIN_URL = "http://play.pokemonshowdown.com/action.php";
    
    private readonly IClient _client;
    private readonly IConfigurationService _configurationService;
    private readonly IHttpService _httpService;

    private readonly IDictionary<string, IRoom> _rooms = new Dictionary<string, IRoom>();
    private string? _currentRoom;
    private bool _disposed;

    public Bot(IClient client, IConfigurationService configurationService, IHttpService httpService)
    {
        _client = client;
        _configurationService = configurationService;
        _httpService = httpService;
    }

    public async Task Start()
    {
        _client.MessageReceived.Subscribe(message => Task.Run(async () => await HandleReceivedMessage(message)));
        await _client.Connect();
    }

    private async Task HandleReceivedMessage(string message)
    {
        Console.WriteLine(message); // TODO: proper logger

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
        
        Console.WriteLine($"[{room}] {line}"); // TODO: proper logger

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
                ParseFormats();
                break;
            case "deinit":
                if (roomId != null && _rooms.ContainsKey(roomId))
                {
                    _rooms.Remove(roomId);
                }
                break;
            case "J":
                if (roomId != null && _rooms.ContainsKey(roomId))
                {
                    _rooms[roomId].AddUser(parts[2]);
                }
                break;
            case "L":
                if (roomId != null && _rooms.ContainsKey(roomId))
                {
                    _rooms[roomId].RemoveUser(parts[2]);
                }
                break;
            case "N":
                if (roomId != null && _rooms.ContainsKey(roomId))
                {
                    _rooms[roomId].RenameUser(parts[3], parts[2]);
                }
                break;
            case "c:":
                await HandleChatMessage(parts[4], parts[3], roomId, int.Parse(parts[2]));
                break;
                
        }
    }

    private async Task HandleChatMessage(string message, string sender, string? roomId, int timestamp)
    {
    }

    private void ParseFormats()
    {
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
            Console.WriteLine($"Connection successful, logged in as {name}");

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
        var parameters = new Dictionary<string, string>
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
        
        Console.WriteLine($"Initializing {roomTitle}..."); // TODO : logger

        var room = new Room(roomTitle, roomId, "fr-FR"); // TODO: factory ?/ locale
        foreach (var user in users)
        {
            room.AddUser(user);
        }
        _rooms[room.RoomId] = room;

        Console.WriteLine($"Initializing {roomTitle} : DONE"); // TODO : logger
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