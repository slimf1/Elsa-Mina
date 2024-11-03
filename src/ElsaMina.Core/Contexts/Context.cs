using System.Globalization;
using ElsaMina.Core.Models;

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
    public string Command { get; }

    public bool IsSenderWhitelisted => _contextProvider.CurrentWhitelist
        .Contains(Sender.UserId);

    public void SendHtmlPage(string pageName, string html)
    {
        Bot.Say(_contextProvider.DefaultRoom,
            $"/sendhtmlpage {Sender.UserId}, {pageName}, {html}");
    }

    public string GetString(string key) => _contextProvider.GetString(key, Culture);

    public string GetString(string key, params object[] formatArguments) =>
        string.Format(GetString(key), formatArguments);

    public void ReplyLocalizedMessage(string key, params object[] formatArguments)
    {
        Reply(GetString(key, formatArguments));
    }

    public void ReplyRankAwareLocalizedMessage(string key, params object[] formatArguments)
    {
        Reply(GetString(key, formatArguments), rankAware: true);
    }

    public override string ToString()
    {
        return $"{nameof(Context)}[{nameof(Bot)}: {Bot}, " +
               $"{nameof(Target)}: {Target}, " +
               $"{nameof(Sender)}: {Sender}, " +
               $"{nameof(Command)}: {Command}, " +
               $"{nameof(IsSenderWhitelisted)}: {IsSenderWhitelisted}, " +
               $"{nameof(RoomId)}: {RoomId}, " +
               $"{nameof(IsPrivateMessage)}: {IsPrivateMessage}, " +
               $"{nameof(Culture)}: {Culture}]";
    }

    public abstract string RoomId { get; }
    public abstract bool IsPrivateMessage { get; }
    public abstract CultureInfo Culture { get; set; }
    public abstract ContextType Type { get; }

    public abstract bool HasSufficientRank(char requiredRank);
    public abstract void Reply(string message, bool rankAware = false);
    public abstract void SendHtml(string html, string roomId = null, bool rankAware = false);
    public abstract void SendUpdatableHtml(string htmlId, string html, bool isChanging);
}