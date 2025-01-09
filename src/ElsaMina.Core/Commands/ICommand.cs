using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;

namespace ElsaMina.Core.Commands;

public interface ICommand
{
    public string Name { get; }
    public IEnumerable<string> Aliases { get; }
    public bool IsAllowedInPrivateMessage { get; }
    public bool IsWhitelistOnly { get; }
    public bool IsPrivateMessageOnly { get; }
    public Rank RequiredRank { get; }
    public string HelpMessageKey { get; }
    public bool IsHidden { get; }

    void ReplyLocalizedHelpMessage(IContext context, params object[] formatArguments);
    Task OnBotStartUp();
    Task Call(IContext context);
}