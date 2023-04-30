
using Autofac;
using ElsaMina.Core.Commands;

namespace ElsaMina.Core.Services.DependencyInjection;

public interface IDependencyContainerService
{
    public IContainer? Container { get; set; }
    public T? Resolve<T>() where T : notnull;
    public T? ResolveCommand<T>(string commandName) where T : ICommand;
    public bool IsCommandRegistered(string commandName);
}