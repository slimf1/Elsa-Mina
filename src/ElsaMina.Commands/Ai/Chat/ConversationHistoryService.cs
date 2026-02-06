using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Ai.Chat;

public sealed class ConversationHistoryService : IConversationHistoryService
{
    private const int MAX_HISTORY_MESSAGES = 20;
    private readonly Dictionary<string, List<LanguageModelMessage>> _historyByRoom = new();
    private readonly IConfiguration _configuration;

    public ConversationHistoryService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<LanguageModelMessage> BuildConversation(IRoom room, IUser sender, string latestMessage)
    {
        var history = GetOrCreateHistory(room);
        List<LanguageModelMessage> snapshot;
        lock (history)
        {
            if (!string.IsNullOrWhiteSpace(latestMessage))
            {
                history.Add(new LanguageModelMessage
                {
                    Role = MessageRole.User,
                    Content = $"{FormatUserName(sender.Name)}: {latestMessage}"
                });
                TrimHistory(history);
            }

            snapshot = history.Select(message => new LanguageModelMessage
            {
                Role = message.Role,
                Content = message.Content
            }).ToList();
        }

        return snapshot;
    }

    public void StoreAssistantReply(IRoom room, string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return;
        }

        var history = GetOrCreateHistory(room);
        lock (history)
        {
            history.Add(new LanguageModelMessage
            {
                Role = MessageRole.Agent,
                Content = response
            });
            TrimHistory(history);
        }
    }

    private List<LanguageModelMessage> GetOrCreateHistory(IRoom room)
    {
        return GetOrCreate(room.RoomId, () => InitializeHistory(room));
    }

    private List<LanguageModelMessage> GetOrCreate(string key, Func<List<LanguageModelMessage>> createHistory)
    {
        // Lists are not thread-safe => prefer lock over ConcurrentDictionary
        lock (_historyByRoom)
        {
            if (_historyByRoom.TryGetValue(key, out var history))
            {
                return history;
            }

            history = createHistory();
            _historyByRoom[key] = history;
            return history;
        }
    }

    private List<LanguageModelMessage> InitializeHistory(IRoom room)
    {
        var history = new List<LanguageModelMessage>();
        var botId = _configuration.Name.ToLowerAlphaNum();

        foreach (var (user, message) in room.LastMessages.Reverse())
        {
            if (string.IsNullOrWhiteSpace(message) || IsCommandMessage(message))
            {
                continue;
            }

            var userId = user.ToLowerAlphaNum();
            var role = userId == botId ? MessageRole.Agent : MessageRole.User;
            history.Add(new LanguageModelMessage
            {
                Role = role,
                Content = role == MessageRole.User ? $"{FormatUserName(user)}: {message}" : message
            });
        }

        TrimHistory(history);
        return history;
    }

    private void TrimHistory(List<LanguageModelMessage> history)
    {
        if (history.Count <= MAX_HISTORY_MESSAGES)
        {
            return;
        }

        var removeCount = history.Count - MAX_HISTORY_MESSAGES;
        history.RemoveRange(0, removeCount);
    }

    private static string FormatUserName(string user)
    {
        return string.IsNullOrEmpty(user)
            ? user
            : user.Length > 1 && !char.IsLetterOrDigit(user[0]) ? user[1..] : user;
    }

    private bool IsCommandMessage(string message)
    {
        var trigger = _configuration.Trigger ?? string.Empty;
        return trigger.Length > 0 && message.StartsWith(trigger, StringComparison.OrdinalIgnoreCase);
    }
}
