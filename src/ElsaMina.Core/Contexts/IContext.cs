using System.Globalization;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Contexts;

public interface IContext
{
    string Message { get; }
    string Target { get; }
    IUser Sender { get; }
    IRoom Room { get; }
    string Command { get; }
    bool IsSenderWhitelisted { get; }
    string RoomId { get; }
    bool IsPrivateMessage { get; }
    CultureInfo Culture { get; set; }
    ContextType Type { get; }

    string GetString(string key);
    string GetString(string key, params object[] formatArguments);
    void SendHtmlPage(string pageName, string html);
    bool HasSufficientRank(Rank requiredRank);
    void Reply(string message, bool rankAware = false);
    void ReplyLocalizedMessage(string key, params object[] formatArguments);
    void ReplyRankAwareLocalizedMessage(string key, params object[] formatArguments);
    void SendHtml(string html, string roomId = null, bool rankAware = false);
    void SendHtmlTo(string userId, string html, string roomId = null);
    void SendUpdatableHtml(string htmlId, string html, bool isChanging);
    void HandleError(Exception exception);
}