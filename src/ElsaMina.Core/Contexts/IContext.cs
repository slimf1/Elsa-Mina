using System.Globalization;
using ElsaMina.Core.Models;

namespace ElsaMina.Core.Contexts;

public interface IContext
{
    string Message { get; }
    string Target { get; }
    IUser Sender { get; }
    string Command { get; }
    bool IsSenderWhitelisted { get; }
    string RoomId { get; }
    bool IsPm { get; }
    CultureInfo Culture { get; set; }

    string GetString(string key);
    string GetString(string key, params object[] formatArguments);
    void SendHtmlPage(string pageName, string html);
    bool HasSufficientRank(char requiredRank, string roomId = "");
    void Reply(string message);
    void ReplyLocalizedMessage(string key, params object[] formatArguments);
    void SendHtml(string html, string roomId = null);
    void SendUpdatableHtml(string htmlId, string html, bool isChanging);
}