using System.Globalization;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;

namespace ElsaMina.Core.Contexts;

public interface IContext
{
    IBot Bot { get; }
    string Target { get; }
    IUser Sender { get; }
    string Command { get; }
    bool IsSenderWhitelisted { get; }
    string RoomId { get; }
    ContextType Type { get; }
    bool IsPm { get; }
    CultureInfo Locale { get; set; }
    
    void SendHtmlPage(string pageName, string html);
    bool HasSufficientRank(char requiredRank);
    void Reply(string message);
    void SendHtml(string html, string roomId = null);
    void SendUpdatableHtml(string htmlId, string html, bool isChanging);
}