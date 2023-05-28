using Autofac;
using ElsaMina.Core.Commands;

namespace ElsaMina.Core.Services.DependencyInjection;

public class DependencyContainerService : IDependencyContainerService
{
    public static IDependencyContainerService s_ContainerService;

    public IContainer Container { get; set; }

    public T Resolve<T>() where T : notnull
    {
        return Container == null ? default : Container.Resolve<T>();
    }

    public T ResolveCommand<T>(string commandName) where T : ICommand
    {
        return Container == null ? default : Container.ResolveNamed<T>(commandName);
    }

    public bool IsCommandRegistered(string commandName)
    {
        return Container?.IsRegisteredWithName<ICommand>(commandName) ?? false;
    }
}