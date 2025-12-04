using System.Collections.Concurrent;
using System.Globalization;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Services.Rooms;

public class Room : IRoom
{
    private const int MESSAGE_QUEUE_LENGTH = 8;

    private readonly ConcurrentDictionary<string, DateTime> _joinDateTimes = [];
    private readonly ConcurrentDictionary<string, TimeSpan> _pendingPlayTimeUpdates = [];
    private readonly Queue<Tuple<string, string>> _lastMessages = new(MESSAGE_QUEUE_LENGTH);
    private readonly ConcurrentDictionary<string, IUser> _users = new();
    private readonly IRoomParameterStore _roomParameterStore;
    private readonly IReadOnlyDictionary<Parameter, IParameterDefiniton> _parametersDefinitions;
    private IGame _game;

    public Room(string roomTitle, string roomId, CultureInfo culture, IRoomParameterStore roomParameterStore,
        IReadOnlyDictionary<Parameter, IParameterDefiniton> parametersDefinitions)
    {
        RoomId = roomId ?? roomTitle.ToLowerAlphaNum();
        Name = roomTitle;
        Culture = culture;

        _roomParameterStore = roomParameterStore;
        _parametersDefinitions = parametersDefinitions;
    }

    public string RoomId { get; }
    public string Name { get; }
    public IReadOnlyDictionary<string, IUser> Users => _users;
    public CultureInfo Culture { get; set; }

    public IGame Game
    {
        get => _game;
        set => OnGameChanged(_game, value);
    }

    public IEnumerable<Tuple<string, string>> LastMessages => _lastMessages.Reverse();
    public IDictionary<string, TimeSpan> PendingPlayTimeUpdates => _pendingPlayTimeUpdates;

    public void UpdateMessageQueue(string user, string message)
    {
        _lastMessages.Enqueue(Tuple.Create(user, message));
        if (_lastMessages.Count == MESSAGE_QUEUE_LENGTH)
        {
            _lastMessages.Dequeue();
        }
    }

    public string GetParameterValue(Parameter parameter)
    {
        var parameterDefinition = _parametersDefinitions[parameter];
        return _roomParameterStore.GetValue(parameter) ?? parameterDefinition.DefaultValue;
    }

    public async Task<string> GetParameterValueAsync(Parameter parameter, CancellationToken cancellationToken = default)
    {
        var parameterDefinition = _parametersDefinitions[parameter];
        return await _roomParameterStore.GetValueAsync(parameter, cancellationToken) ?? parameterDefinition.DefaultValue;
    }

    public bool SetParameterValue(Parameter parameter, string value)
    {
        return _roomParameterStore.SetValue(parameter, value);
    }

    public async Task<bool> SetParameterValueAsync(Parameter parameter, string value,
        CancellationToken cancellationToken = default)
    {
        return await _roomParameterStore.SetValueAsync(parameter, value, cancellationToken);
    }

    public void InitializeMessageQueueFromLogs(IEnumerable<string> logs)
    {
        var filteredMessages = logs
            .Where(line => line.StartsWith("|c:|"))
            .TakeLast(MESSAGE_QUEUE_LENGTH)
            .Select(line => line.Split("|"))
            .Where(messageParts => !messageParts[4].StartsWith("/raw"))
            .Select(messageParts => (messageParts[3], messageParts[4]));

        foreach (var (user, message) in filteredMessages)
        {
            _lastMessages.Enqueue(Tuple.Create(user, message));
        }
    }

    public void AddUser(string username)
    {
        var user = User.FromUsername(username);
        _users[user.UserId] = user;
        _joinDateTimes[user.UserId] = DateTime.UtcNow;
    }

    public void RemoveUser(string username)
    {
        var userId = username.ToLowerAlphaNum();
        _users.Remove(userId, out _);

        if (!_joinDateTimes.Remove(userId, out var joinTime))
        {
            return;
        }

        var playTime = DateTime.UtcNow - joinTime;
        if (!_pendingPlayTimeUpdates.TryAdd(userId, playTime))
        {
            _pendingPlayTimeUpdates[userId] += playTime;
        }
    }

    public void RenameUser(string oldName, string newName)
    {
        RemoveUser(oldName);
        AddUser(newName);
    }

    public void AddUsers(IEnumerable<string> users)
    {
        foreach (var user in users)
        {
            AddUser(user);
        }
    }

    private void OnGameChanged(IGame oldGame, IGame newGame)
    {
        if (oldGame != null)
        {
            oldGame.GameStarted -= HandleGameStart;
            oldGame.GameEnded -= HandleGameEnd;
        }

        if (newGame != null)
        {
            newGame.GameStarted += HandleGameStart;
            newGame.GameEnded += HandleGameEnd;
        }

        _game = newGame;
    }

    private void HandleGameEnd()
    {
        Game = null;
    }

    private void HandleGameStart()
    {
        // Do nothing
    }
}