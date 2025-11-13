using Autofac;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Handlers;
using ElsaMina.Logging;

namespace ElsaMina.Core.Utils;

public static class DependencyInjection
{
    public static void RegisterCommand<TCommand>(this ContainerBuilder builder) where TCommand : ICommand
    {
        var commandAttribute = typeof(TCommand).GetCommandAttribute();
        if (commandAttribute == null)
        {
            Log.Warning(
                "Command '{0}' does not have the named command attribute, and could not be registered",
                typeof(TCommand).Name);
            return;
        }

        var commandName = commandAttribute.Name;
        if (string.IsNullOrEmpty(commandName))
        {
            Log.Warning("Command '{0}' has no name, and could not be registered", typeof(TCommand).Name);
            return;
        }

        Log.Information("Command '{0}' was registered", commandName);
        builder.RegisterType<TCommand>().Named<ICommand>(commandName);
        foreach (var commandAlias in commandAttribute.Aliases ?? Enumerable.Empty<string>())
        {
            Log.Information("Alias '{0}' of command '{1}' was registered", commandAlias, commandName);
            builder.RegisterType<TCommand>().Named<ICommand>(commandAlias);
        }
    }

    public static void RegisterHandler<THandler>(this ContainerBuilder builder) where THandler : IHandler
    {
        builder.RegisterType<THandler>().As<IHandler>().SingleInstance();
    }
}