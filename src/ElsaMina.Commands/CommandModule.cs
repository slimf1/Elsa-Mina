using Autofac;
using ElsaMina.Commands.Badges;
using ElsaMina.Commands.CustomCommands;
using ElsaMina.Commands.Development;
using ElsaMina.Commands.GuessingGame;
using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Commands.Misc.Repeats;
using ElsaMina.Commands.Profile;
using ElsaMina.Commands.RoomDashboard;
using ElsaMina.Commands.Teams.Samples;
using ElsaMina.Commands.Teams.TeamPreviewOnLink;
using ElsaMina.Commands.Teams.TeamPreviewOnLink.TeamProviders.CoupCritique;
using ElsaMina.Commands.Teams.TeamPreviewOnLink.TeamProviders.Pokepaste;
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
        
        RegisterParser<JoinRoomOnInviteParser>(builder);
        RegisterParser<GuessingGameParser>(builder);
        RegisterParser<DisplayTeamOnLinkParser>(builder);
        
        builder.RegisterType<CountriesGame>().AsSelf();
        
        builder.RegisterType<PokepasteProvider>().AsSelf();
        builder.RegisterType<CoupCritiqueProvider>().AsSelf();
        builder.RegisterType<TeamProviderFactory>().As<ITeamProviderFactory>().SingleInstance();
    }

    private static void RegisterCommand<T>(ContainerBuilder builder) where T : INamed, ICommand
    {
        var commandName = T.Name;
        if (string.IsNullOrEmpty(commandName))
        {
            Console.WriteLine("[WARN] Command "+ typeof(T).Name + " has no name, and could not be registered");
            return;
        }
        Console.WriteLine("Command "+ typeof(T).Name + " was registered");
        builder.RegisterType<T>().AsSelf().Named<ICommand>(commandName);
        foreach (var alias in T.Aliases)
        {
            builder.RegisterType<T>().AsSelf().Named<ICommand>(alias);
        }
    }

    private static void RegisterParser<T>(ContainerBuilder builder) where T : IParser
    {
        builder.RegisterType<T>().As<IParser>().SingleInstance();
    }
}