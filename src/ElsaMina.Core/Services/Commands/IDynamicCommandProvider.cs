using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Commands;

public interface IDynamicCommandProvider
{
    Task<bool> TryExecuteAsync(string commandName, IContext context);
}
