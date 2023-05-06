using Autofac;
using ElsaMina.Commands.CustomCommands;
using ElsaMina.Commands.Development;
using ElsaMina.Core.Commands;

namespace ElsaMina.Commands;

public class CommandModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        RegisterCommand<Ping>(builder);
        RegisterCommand<AddCustomCommand>(builder);
        RegisterCommand<CustomCommandList>(builder);
    }

    private static void RegisterCommand<T>(ContainerBuilder builder) where T : ICommand
    {
        var commandName = T.Name;
        if (string.IsNullOrEmpty(commandName))
        {
            Console.WriteLine("[WARN] Command "+ typeof(T).Name + " has no name, and could not be registered");
            return;
        }
        builder.RegisterType<T>().Named<ICommand>(commandName);
        foreach (var alias in T.Aliases)
        {
            builder.RegisterType<T>().Named<ICommand>(alias);
        }
    }
}