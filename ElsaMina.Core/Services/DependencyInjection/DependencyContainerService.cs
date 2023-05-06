
using Autofac;
using ElsaMina.Core.Commands;

namespace ElsaMina.Core.Services.DependencyInjection;

public interface IDependencyContainerService
{
    IContainer Container { get; set; }
    T Resolve<T>() where T : notnull;
    T ResolveCommand<T>(string commandName) where T : ICommand;
    bool IsCommandRegistered(string commandName);
}
