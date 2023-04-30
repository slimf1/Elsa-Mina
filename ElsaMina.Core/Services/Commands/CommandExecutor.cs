using ElsaMina.Core.Contexts;
using ElsaMina.Core.Modules;

namespace ElsaMina.Core.Services.Commands;

public class CommandExecutor : ICommandExecutor
{
    public bool HasCommand(string commandName)
    {
        return CoreModule.IsCommandRegistered(commandName);
    }
    
    public async Task TryExecuteCommand(string commandName, Context context)
    {
        var commandInstance = CoreModule.ResolveCommand(commandName);
        await commandInstance.Call(context);
    }
}