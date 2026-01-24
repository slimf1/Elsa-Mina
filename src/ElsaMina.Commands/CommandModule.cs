using Autofac;
using ElsaMina.Commands.Ai.Chat;
using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Commands.Ai.LanguageModel.Google;
using ElsaMina.Commands.Ai.LanguageModel.Mistral;
using ElsaMina.Commands.Ai.LanguageModel.OpenAi;
using ElsaMina.Commands.Ai.TextToSpeech;
using ElsaMina.Commands.Arcade;
using ElsaMina.Commands.Arcade.Events;
using ElsaMina.Commands.Arcade.Levels;
using ElsaMina.Commands.Arcade.Sheets;
using ElsaMina.Commands.Badges;
using ElsaMina.Commands.Badges.BadgeDisplay;
using ElsaMina.Commands.Badges.BadgeHolders;
using ElsaMina.Commands.Badges.HallOfFame;
using ElsaMina.Commands.ConnectFour;
using ElsaMina.Commands.CustomCommands;
using ElsaMina.Commands.Development;
using ElsaMina.Commands.Development.Commands;
using ElsaMina.Commands.GuessingGame;
using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Commands.GuessingGame.Gatekeepers;
using ElsaMina.Commands.GuessingGame.PokeCries;
using ElsaMina.Commands.GuessingGame.PokeDesc;
using ElsaMina.Commands.JoinPhrases;
using ElsaMina.Commands.Misc;
using ElsaMina.Commands.Misc.Bitcoin;
using ElsaMina.Commands.Misc.Colors;
using ElsaMina.Commands.Misc.Dailymotion;
using ElsaMina.Commands.Misc.Facts;
using ElsaMina.Commands.Misc.Genius;
using ElsaMina.Commands.Misc.Pairings;
using ElsaMina.Commands.Misc.Pokemon;
using ElsaMina.Commands.Misc.Wiki;
using ElsaMina.Commands.Misc.Youtube;
using ElsaMina.Commands.Polls;
using ElsaMina.Commands.Profile;
using ElsaMina.Commands.Repeats;
using ElsaMina.Commands.Repeats.Form;
using ElsaMina.Commands.Repeats.List;
using ElsaMina.Commands.Replays;
using ElsaMina.Commands.RoomDashboard;
using ElsaMina.Commands.Showdown.Ladder;
using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Commands.Teams.Samples;
using ElsaMina.Commands.Teams.TeamPreviewOnLink;
using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Commands.Teams.TeamProviders.CoupCritique;
using ElsaMina.Commands.Teams.TeamProviders.Pokepaste;
using ElsaMina.Commands.Tournaments;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands;

public class CommandModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
#if DEBUG
        builder.RegisterCommand<ScriptCommand>();
#endif
        builder.RegisterCommand<Ping>();
        builder.RegisterCommand<AddCustomCommand>();
        builder.RegisterCommand<CustomCommandList>();
        builder.RegisterCommand<AddBadgeCommand>();
        builder.RegisterCommand<SetLocaleCommand>();
        builder.RegisterCommand<HelpCommand>();
        builder.RegisterCommand<ShowRoomDashboard>();
        builder.RegisterCommand<RoomConfigCommand>();
        builder.RegisterCommand<KillCommand>();
        builder.RegisterCommand<StopConnectionCommand>();
        builder.RegisterCommand<DeleteCustomCommand>();
        builder.RegisterCommand<EditCustomCommand>();
        builder.RegisterCommand<TemplatesDebugCommand>();
        builder.RegisterCommand<GiveBadgeCommand>();
        builder.RegisterCommand<ProfileCommand>();
        builder.RegisterCommand<DeleteBadgeCommand>();
        builder.RegisterCommand<TakeBadgeCommand>();
        builder.RegisterCommand<SetAvatarCommand>();
        builder.RegisterCommand<SetTitleCommand>();
        builder.RegisterCommand<AllCommands>();
        builder.RegisterCommand<GuessingGameCommand>();
        builder.RegisterCommand<EndGuessingGameCommand>();
        builder.RegisterCommand<AddTeamCommand>();
        builder.RegisterCommand<AddTeamToRoomCommand>();
        builder.RegisterCommand<TeamShowcaseCommand>();
        builder.RegisterCommand<TeamListCommand>();
        builder.RegisterCommand<DeleteTeamCommand>();
        builder.RegisterCommand<SayCommand>();
        builder.RegisterCommand<NameColorInfoCommand>();
        builder.RegisterCommand<FactsCommand>();
        builder.RegisterCommand<BitcoinCommand>();
        builder.RegisterCommand<SetJoinPhraseCommand>();
        builder.RegisterCommand<CreateConnectFourCommand>();
        builder.RegisterCommand<JoinConnectFourCommand>();
        builder.RegisterCommand<PlayConnectFourCommand>();
        builder.RegisterCommand<EndConnectFourCommand>();
        builder.RegisterCommand<YoutubeCommand>();
        builder.RegisterCommand<SetArcadeLevelCommand>();
        builder.RegisterCommand<DisplayArcadeLevelsCommand>();
        builder.RegisterCommand<DeleteArcadeLevelCommand>();
        builder.RegisterCommand<ForfeitConnectFourCommand>();
        builder.RegisterCommand<AskElsaCommand>();
        builder.RegisterCommand<SpeakCommand>();
        builder.RegisterCommand<RankingCommand>();
        builder.RegisterCommand<LadderCommand>();
        builder.RegisterCommand<FailCommand>();
        builder.RegisterCommand<RepeatFormCommand>();
        builder.RegisterCommand<StartRepeatCommand>();
        builder.RegisterCommand<StopRepeatCommand>();
        builder.RegisterCommand<RepeatsListCommand>();
        builder.RegisterCommand<WikipediaSearchCommand>();
        builder.RegisterCommand<AfdSpriteCommand>();
        builder.RegisterCommand<ShowPollsCommand>();
        builder.RegisterCommand<PairingsCommand>();
        builder.RegisterCommand<EvroMakerCommand>();
        builder.RegisterCommand<DailymotionCommand>();
        builder.RegisterCommand<MemoryUsageCommand>();
        builder.RegisterCommand<UptimeCommand>();
        builder.RegisterCommand<BadgeDisplayCommand>();
        builder.RegisterCommand<BadgeHoldersCommand>();
        builder.RegisterCommand<HallOfFameCommand>();
        builder.RegisterCommand<GeniusSearchCommand>();
        builder.RegisterCommand<ArcadeHallOfFameCommand>();
        builder.RegisterCommand<TimerCommand>();
        builder.RegisterCommand<RunningCommands>();
        builder.RegisterCommand<CancelRunningCommand>();

        builder.RegisterHandler<JoinRoomOnInviteHandler>();
        builder.RegisterHandler<GuessingGameHandler>();
        builder.RegisterHandler<DisplayTeamOnLinkHandler>();
        builder.RegisterHandler<JoinPhraseHandler>();
        builder.RegisterHandler<ReplaysHandler>();
        builder.RegisterHandler<DisplayTeamsOnTourHandler>();
        builder.RegisterHandler<TourFinaleAnnounceHandler>();
        builder.RegisterHandler<ArcadeEventsHandler>();
        builder.RegisterHandler<PollEndHandler>();
        builder.RegisterHandler<OtherRoomTournamentAnnounceHandler>();

        builder.RegisterType<CountriesGame>().AsSelf();
        builder.RegisterType<ConnectFourGame>().AsSelf();
        builder.RegisterType<PokeDescGame>().AsSelf();
        builder.RegisterType<PokeCriesGame>().AsSelf();
        builder.RegisterType<GatekeepersGame>().AsSelf();

        builder.RegisterType<ElevenLabsAiTextToSpeechProvider>().As<IAiTextToSpeechProvider>().SingleInstance();
        //builder.RegisterType<MistralLanguageModelProvider>().As<ILanguageModelProvider>().SingleInstance();
        //builder.RegisterType<GptLanguageModelProvider>().As<ILanguageModelProvider>().SingleInstance();
        builder.RegisterType<GeminiLanguageModelProvider>().As<ILanguageModelProvider>().SingleInstance();
        builder.RegisterType<ShowdownRanksProvider>().As<IShowdownRanksProvider>().SingleInstance();
        builder.RegisterType<PokepasteProvider>().As<ITeamProvider>().SingleInstance();
        builder.RegisterType<CoupCritiqueProvider>().As<ITeamProvider>().SingleInstance();
        builder.RegisterType<TeamLinkMatchFactory>().As<ITeamLinkMatchFactory>().SingleInstance();
        builder.RegisterType<DataManager>().As<IDataManager>().SingleInstance().OnActivating(e =>
        {
            e.Instance.Initialize().Wait(); // Risque d'ANR mais obligé pour garantir la bonne initialisation...
        });
    }
}
