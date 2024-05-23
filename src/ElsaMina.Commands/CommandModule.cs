using Autofac;
using ElsaMina.Commands.Badges;
using ElsaMina.Commands.CustomCommands;
using ElsaMina.Commands.Development;
using ElsaMina.Commands.Development.Commands;
using ElsaMina.Commands.GuessingGame;
using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Commands.Misc.Repeats;
using ElsaMina.Commands.Profile;
using ElsaMina.Commands.RoomDashboard;
using ElsaMina.Commands.Teams.Samples;
using ElsaMina.Commands.Teams.TeamPreviewOnLink;
using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Commands.Teams.TeamProviders.CoupCritique;
using ElsaMina.Commands.Teams.TeamProviders.Pokepaste;
using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands;

public class CommandModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        RegisterCommand<Ping>(builder);
        RegisterCommand<AddCustomCommand>(builder);
        RegisterCommand<CustomCommandList>(builder);
        RegisterCommand<AddBadge>(builder);
        RegisterCommand<SetLocale>(builder);
        RegisterCommand<Help>(builder);
        RegisterCommand<ShowRoomDashboard>(builder);
        RegisterCommand<RoomConfig>(builder);
        RegisterCommand<Kill>(builder);
        RegisterCommand<StopConnection>(builder);
        RegisterCommand<Script>(builder);
        RegisterCommand<DeleteCustomCommand>(builder);
        RegisterCommand<EditCustomCommand>(builder);
        RegisterCommand<TemplatesDebug>(builder);
        RegisterCommand<GiveBadge>(builder);
        RegisterCommand<ProfileCommand>(builder);
        RegisterCommand<DeleteBadge>(builder);
        RegisterCommand<TakeBadge>(builder);
        RegisterCommand<SetAvatar>(builder);
        RegisterCommand<SetTitle>(builder);
        RegisterCommand<AllCommands>(builder);
        RegisterCommand<GuessingGameCommand>(builder);
        RegisterCommand<EndGuessingGame>(builder);
        RegisterCommand<AddTeam>(builder);
        RegisterCommand<AddTeamToRoom>(builder);
        RegisterCommand<TeamShowcase>(builder);
        RegisterCommand<TeamList>(builder);
        RegisterCommand<DeleteTeam>(builder);
        RegisterCommand<AboutRepeat>(builder);
        RegisterCommand<CreateRepeat>(builder);
        RegisterCommand<StopRepeat>(builder);
        RegisterCommand<Say>(builder);

        RegisterParser<JoinRoomOnInviteHandler>(builder);
        RegisterParser<GuessingGameHandler>(builder);
        RegisterParser<DisplayTeamOnLinkHandler>(builder);

        builder.RegisterType<CountriesGame>().AsSelf();

        builder.RegisterType<PokepasteProvider>().As<ITeamProvider>();
        builder.RegisterType<CoupCritiqueProvider>().As<ITeamProvider>();
        builder.RegisterType<TeamLinkMatchFactory>().As<ITeamLinkMatchFactory>().SingleInstance();
    }

    private static void RegisterCommand<T>(ContainerBuilder builder) where T : ICommand
    {
        var commandAttribute = typeof(T).GetCommandAttribute();
        if (commandAttribute == null)
        {
            Logger.Current.Warning(
                "Command '{0}' does not have the named command attribute, and could not be registered",
                typeof(T).Name);
            return;
        }
        
        var commandName = commandAttribute.Name;
        if (string.IsNullOrEmpty(commandName))
        {
            Logger.Current.Warning("Command '{0}' has no name, and could not be registered", typeof(T).Name);
            return;
        }

        Logger.Current.Information("Command '{0}' was registered", commandName);
        builder.RegisterType<T>().AsSelf().Named<ICommand>(commandName);
        foreach (var commandAlias in commandAttribute.Aliases ?? Enumerable.Empty<string>())
        {
            Logger.Current.Information("Alias '{0}' of command '{1}' was registered", commandAlias, commandName);
            builder.RegisterType<T>().AsSelf().Named<ICommand>(commandAlias);
        }
    }

    private static void RegisterParser<T>(ContainerBuilder builder) where T : IHandler
    {
        builder.RegisterType<T>().As<IHandler>().SingleInstance();
    }
}