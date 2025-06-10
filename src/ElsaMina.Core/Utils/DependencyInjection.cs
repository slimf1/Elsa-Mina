using System.Reflection;
using Autofac;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Handlers;

namespace ElsaMina.Core.Utils;

public static class DependencyInjection
{
    public static void RegisterCommand<TCommand>(this ContainerBuilder builder) where TCommand : ICommand
    {
        builder.RegisterCommand(typeof(TCommand));
    }

    public static void RegisterCommand(this ContainerBuilder builder, Type type)
    {
        var commandAttribute = type.GetCommandAttribute();
        if (commandAttribute == null)
        {
            Log.Warning(
                "Command '{0}' does not have the named command attribute, and could not be registered",
                type.Name);
            return;
        }

        var commandName = commandAttribute.Name;
        if (string.IsNullOrEmpty(commandName))
        {
            Log.Warning("Command '{0}' has no name, and could not be registered", type.Name);
            return;
        }

        Log.Information("Command '{0}' was registered", commandName);
        builder.RegisterType(type).Named<ICommand>(commandName);
        foreach (var commandAlias in commandAttribute.Aliases ?? Enumerable.Empty<string>())
        {
            Log.Information("Alias '{0}' of command '{1}' was registered", commandAlias, commandName);
            builder.RegisterType(type).Named<ICommand>(commandAlias);
        }
    }

    public static void RegisterFromAssembly(this ContainerBuilder builder, Assembly assembly)
    {
        assembly
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Command)))
            .Where(type => type.GetCustomAttribute(typeof(NamedCommandAttribute), false) != null)
            .ToList()
            .ForEach(builder.RegisterCommand);

        assembly
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Handler)))
            .Where(type => typeof(IHandler).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass)
            .ToList()
            .ForEach(builder.RegisterHandler);
    }

    public static void RegisterHandler<THandler>(this ContainerBuilder builder) where THandler : IHandler
    {
        builder.RegisterHandler(typeof(THandler));
    }

    public static void RegisterHandler(this ContainerBuilder builder, Type type)
    {
        builder.RegisterType(type).As<IHandler>().SingleInstance();
        Log.Information("Handler '{0}' was registered", type.Name);
    }
}