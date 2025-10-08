using System.Globalization;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Contexts;

public abstract class Context : IContext
{
    private readonly IContextProvider _contextProvider;

    protected Context(IContextProvider contextProvider,
        IBot bot,
        string message,
        string target,
        IUser sender,
        string command)
    {
        _contextProvider = contextProvider;

        Bot = bot;
        Message = message;
        Target = target;
        Sender = sender;
        Command = command;
    }

    public IBot Bot { get; }
    public string Message { get; }
    public string Target { get; }
    public IUser Sender { get; }
    public IRoom Room => _contextProvider.GetRoom(RoomId);
    public string Command { get; }
    public bool IsSenderWhitelisted => _contextProvider.IsUserWhitelisted(Sender.UserId);

    public void ReplyHtmlPage(string pageName, string html)
    {
        Bot.Say(_contextProvider.DefaultRoom,
            $"/sendhtmlpage {Sender.UserId}, {pageName}, {html}");
    }

    public void SendMessageIn(string roomId, string message)
    {
        Bot.Say(roomId, message);
    }

    public void SendHtmlPageTo(string userId, string pageName, string html)
    {
        Bot.Say(_contextProvider.DefaultRoom, $"/sendhtmlpage {userId}, {pageName}, {html}");
    }

    public string GetString(string key) => _contextProvider.GetString(key, Culture);

    public string GetString(string key, params object[] formatArguments)
    {
        if (formatArguments == null || formatArguments.Length == 0)
        {
            return GetString(key);
        }
        return string.Format(GetString(key), formatArguments);
    }

    public void ReplyLocalizedMessage(string key, params object[] formatArguments)
    {
        Reply(GetString(key, formatArguments));
    }

    public void ReplyRankAwareLocalizedMessage(string key, params object[] formatArguments)
    {
        Reply(GetString(key, formatArguments), rankAware: true);
    }

    public void HandleError(Exception exception)
    {
        if (!IsAllowingErrorMessages)
        {
            return;
        }

        ReplyLocalizedMessage("command_execution_error");
        Reply($"!code {exception.GetType().FullName}: {exception.Message}\n{exception.StackTrace}");
    }

    public Task<Rank> GetUserRankInRoom(string roomId, CancellationToken cancellationToken = default)
    {
        return _contextProvider.GetUserRankInRoom(roomId, Sender.UserId, cancellationToken);
    }

    public async Task<bool> HasSufficientRankInRoom(string roomId, Rank requiredRank,
        CancellationToken cancellationToken = default)
    {
        if (IsSenderWhitelisted)
        {
            return true;
        }

        var rank = await GetUserRankInRoom(roomId, cancellationToken);
        return rank >= requiredRank;
    }

    public abstract string RoomId { get; }
    public abstract bool IsPrivateMessage { get; }
    public abstract CultureInfo Culture { get; set; }
    public abstract ContextType Type { get; }
    protected abstract bool IsAllowingErrorMessages { get; }

    public abstract bool HasRankOrHigher(Rank requiredRank);
    public abstract void Reply(string message, bool rankAware = false);
    public abstract void SendHtmlIn(string html, string roomId = null, bool rankAware = false);
    public abstract void SendHtmlTo(string userId, string html, string roomId = null);
    public abstract void SendUpdatableHtml(string htmlId, string html, bool isChanging);
}