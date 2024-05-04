using System.Reflection;
using Autofac;
using ElsaMina.Commands.Development;
using ElsaMina.Commands.GuessingGame;
using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Commands.Teams.TeamPreviewOnLink;
using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Commands.Teams.TeamProviders.CoupCritique;
using ElsaMina.Commands.Teams.TeamProviders.Pokepaste;
using ElsaMina.Core;
using ElsaMina.Core.Commands;
using Module = Autofac.Module;

namespace ElsaMina.Commands;

public class CommandModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        RegisterCommandsFromAssembly(builder, typeof(CommandModule).Assembly);

        RegisterParser<JoinRoomOnInviteParser>(builder);
        RegisterParser<GuessingGameParser>(builder);
        RegisterParser<DisplayTeamOnLinkParser>(builder);

        builder.RegisterType<CountriesGame>().AsSelf();

        builder.RegisterType<PokepasteProvider>().As<ITeamProvider>();
        builder.RegisterType<CoupCritiqueProvider>().As<ITeamProvider>();
        builder.RegisterType<TeamLinkMatchFactory>().As<ITeamLinkMatchFactory>().SingleInstance();
    }

    private static void RegisterCommandsFromAssembly(ContainerBuilder builder, Assembly assembly)
    {
        var registerableCommandTypes = assembly
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Command)))
            .Where(type => type.GetCustomAttribute(typeof(NamedCommandAttribute), false) != null);

        foreach (var type in registerableCommandTypes)
        {
            RegisterCommand(builder, type);
        }
    }

    private static void RegisterCommand(ContainerBuilder builder, Type commandType)
    {
        if (commandType.GetCustomAttributes(typeof(NamedCommandAttribute), false).FirstOrDefault()
            is not NamedCommandAttribute commandAttribute)
        {
            Logger.Current.Warning(
                "Command '{0}' does not have the named command attribute, and could not be registered",
                commandType.Name);
            return;
        }
        
        var commandName = commandAttribute.Name;
        if (string.IsNullOrEmpty(commandName))
        {
            Logger.Current.Warning("Command '{0}' has no name, and could not be registered", commandType.Name);
            return;
        }

        Logger.Current.Information("Command '{0}' was registered", commandName);
        builder.RegisterType(commandType).AsSelf().Named<ICommand>(commandName);
        foreach (var commandAlias in commandAttribute.Aliases ?? Enumerable.Empty<string>())
        {
            Logger.Current.Information("Alias '{0}' of command '{1}' was registered", commandAlias, commandName);
            builder.RegisterType(commandType).AsSelf().Named<ICommand>(commandAlias);
        }
    }

    private static void RegisterParser<T>(ContainerBuilder builder) where T : IParser
    {
        builder.RegisterType<T>().As<IParser>().SingleInstance();
    }
}