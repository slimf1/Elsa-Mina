using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands;

public interface ICommand
{
    public string CommandName { get; }
    public IEnumerable<string> CommandAliases { get; }
    public bool IsAllowedInPrivateMessage { get; }
    public bool IsWhitelistOnly { get; }
    public bool IsPrivateMessageOnly { get; }
    public char RequiredRank { get; }
    public string HelpMessageKey { get; }
    public bool IsHidden { get; }

    void ReplyLocalizedHelpMessage(IContext context, params object[] formatArguments);
    Task OnBotStartUp();
    Task Call(IContext context);
}