using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Commands;

public interface ICommandExecutor
{
    IEnumerable<ICommand> GetAllCommands();
    Task OnBotStartUp();
    Task TryExecuteCommand(string commandName, IContext context);
}