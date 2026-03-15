using Autofac;
using ElsaMina.Commands.Ai.Chat;
using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Commands.Ai.LanguageModel.Google;
using ElsaMina.Commands.Ai.LanguageModel.Mistral;
using ElsaMina.Commands.Ai.LanguageModel.OpenAi;
using ElsaMina.Commands.Ai.TextToSpeech;
using ElsaMina.Commands.Arcade.Events;
using ElsaMina.Commands.Arcade.Inscriptions;
using ElsaMina.Commands.Arcade.Levels;
using ElsaMina.Commands.Arcade.Points;
using ElsaMina.Commands.Arcade.Sheets;
using ElsaMina.Commands.Arcade.Slots;
using ElsaMina.Commands.Badges;
using ElsaMina.Commands.Badges.BadgeDisplay;
using ElsaMina.Commands.Badges.BadgeEditPanel;
using ElsaMina.Commands.Badges.BadgeHolders;
using ElsaMina.Commands.Badges.BadgeList;
using ElsaMina.Commands.Badges.HallOfFame;
using ElsaMina.Commands.ConnectFour;
using ElsaMina.Commands.CustomCommands;
using ElsaMina.Commands.Development;
using ElsaMina.Commands.Development.Commands;
using ElsaMina.Commands.GuessingGame;
using ElsaMina.Commands.GuessingGame.Capitals;
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
using ElsaMina.Commands.Misc.Legacy;
using ElsaMina.Commands.Misc.Pairings;
using ElsaMina.Commands.Misc.Pokemon;
using ElsaMina.Commands.Misc.RandomImages;
using ElsaMina.Commands.Misc.Wiki;
using ElsaMina.Commands.Misc.Youtube;
using ElsaMina.Commands.Polls;
using ElsaMina.Commands.Profile;
using ElsaMina.Commands.Repeats;
using ElsaMina.Commands.Repeats.Form;
using ElsaMina.Commands.Repeats.List;
using ElsaMina.Commands.Replays;
using ElsaMina.Commands.RoomDashboard;
using ElsaMina.Commands.Showdown.BattleTracker;
using ElsaMina.Commands.Showdown.Ladder;
using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Commands.Showdown.Searching;
using ElsaMina.Commands.Teams.Samples;
using ElsaMina.Commands.Teams.TeamPreviewOnLink;
using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Commands.Teams.TeamProviders.CoupCritique;
using ElsaMina.Commands.Teams.TeamProviders.Pokepaste;
using ElsaMina.Commands.Shop;
using ElsaMina.Commands.Tournaments;
using ElsaMina.Commands.Users;
using ElsaMina.Commands.PokeRace;
using ElsaMina.Commands.Tournaments.Hebdo;
using ElsaMina.Commands.VoltorbFlip;
using ElsaMina.Commands.Users.PlayTimes;
using ElsaMina.Commands.Watchlist;
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
        builder.RegisterCommand<BadgeEditPanelCommand>();
        builder.RegisterCommand<EditBadgeCommand>();
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
        builder.RegisterCommand<GetArcadeLevelCommand>();
        builder.RegisterCommand<AddPointsCommand>();
        builder.RegisterCommand<RemovePointsCommand>();
        builder.RegisterCommand<LeaderboardCommand>();
        builder.RegisterCommand<ClearPointsCommand>();
        builder.RegisterCommand<ForfeitConnectFourCommand>();
        builder.RegisterCommand<AskElsaCommand>();
        builder.RegisterCommand<SpeakCommand>();
        builder.RegisterCommand<RankingCommand>();
        builder.RegisterCommand<LadderCommand>();
        builder.RegisterCommand<SearchCommand>();
        builder.RegisterCommand<ToggleLadderTrackerCommand>();
        builder.RegisterCommand<FailCommand>();
        builder.RegisterCommand<RepeatFormCommand>();
        builder.RegisterCommand<StartRepeatCommand>();
        builder.RegisterCommand<StopRepeatCommand>();
        builder.RegisterCommand<RepeatsListCommand>();
        builder.RegisterCommand<WikipediaSearchCommand>();
        builder.RegisterCommand<PokepediaSearchCommand>();
        builder.RegisterCommand<BulbapediaSearchCommand>();
        builder.RegisterCommand<AfdSpriteCommand>();
        builder.RegisterCommand<ShowPollsCommand>();
        builder.RegisterCommand<PairingsCommand>();
        builder.RegisterCommand<EvroMakerCommand>();
        builder.RegisterCommand<DebilifyCommand>();
        builder.RegisterCommand<WeebifyCommand>();
        builder.RegisterCommand<ElectionCommand>();
        builder.RegisterCommand<DailymotionCommand>();
        builder.RegisterCommand<MemoryUsageCommand>();
        builder.RegisterCommand<UptimeCommand>();
        builder.RegisterCommand<BadgeDisplayCommand>();
        builder.RegisterCommand<BadgeHoldersCommand>();
        builder.RegisterCommand<BadgeListCommand>();
        builder.RegisterCommand<HallOfFameCommand>();
        builder.RegisterCommand<GeniusSearchCommand>();
        builder.RegisterCommand<ArcadeHallOfFameCommand>();
        builder.RegisterCommand<ArcadeSheetAddPointsCommand>();
        builder.RegisterCommand<ArcadeInCommand>();
        builder.RegisterCommand<ArcadeLeaveCommand>();
        builder.RegisterCommand<ArcadeStartCommand>();
        builder.RegisterCommand<ArcadeStopCommand>();
        builder.RegisterCommand<ArcadeListCommand>();
        builder.RegisterCommand<ArcadeRemoveCommand>();
        builder.RegisterCommand<ArcadeTimerCommand>();
        builder.RegisterCommand<ArcadeAddCommand>();
        builder.RegisterCommand<SlotsFunCommand>();
        builder.RegisterCommand<TimerCommand>();
        builder.RegisterCommand<RunningCommands>();
        builder.RegisterCommand<CancelRunningCommand>();
        builder.RegisterCommand<SearchCommand>();
        builder.RegisterCommand<SeenCommand>();
        builder.RegisterCommand<AltsCommand>();
        builder.RegisterCommand<TopPlayTimesCommand>();
        builder.RegisterCommand<PlayTimeCommand>();
        builder.RegisterCommand<CurrentLadderTrackersCommand>();
        builder.RegisterCommand<GuessingGameAnswerCommand>();
        builder.RegisterCommand<TopTournamentPlayersCommand>();
        builder.RegisterCommand<RandomTournamentCommand>();
        builder.RegisterCommand<ArcadeSheetAddPointsCommand>();
        builder.RegisterCommand<ArcadePointsCommand>();
        builder.RegisterCommand<StartVoltorbFlipCommand>();
        builder.RegisterCommand<JoinVoltorbFlipCommand>();
        builder.RegisterCommand<FlipVoltorbFlipCommand>();
        builder.RegisterCommand<ToggleMarkVoltorbFlipCommand>();
        builder.RegisterCommand<QuitVoltorbFlipCommand>();
        builder.RegisterCommand<EndVoltorbFlipCommand>();
        builder.RegisterCommand<StartPokeRaceCommand>();
        builder.RegisterCommand<JoinPokeRaceCommand>();
        builder.RegisterCommand<StartRaceCommand>();
        builder.RegisterCommand<EndRaceCommand>();

        RegisterRandomImagesCommands(builder);
        RegisterTournamentCommands(builder);
        RegisterShopCommands(builder);

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
        builder.RegisterHandler<TourEndHandler>();
        builder.RegisterHandler<StaffIntroChangeHandler>();
        builder.RegisterHandler<StaffIntroContentHandler>();

        builder.RegisterCommand<AddWatchlistCommand>();
        builder.RegisterCommand<RemoveWatchlistCommand>();

        builder.RegisterType<WatchlistService>().As<IWatchlistService>().SingleInstance();

        builder.RegisterType<CountriesGame>().AsSelf();
        builder.RegisterType<CapitalCitiesGame>().AsSelf();
        builder.RegisterType<ConnectFourGame>().AsSelf();
        builder.RegisterType<VoltorbFlipGame>().AsSelf();
        builder.RegisterType<PokeRaceGame>().AsSelf();
        builder.RegisterType<PokeDescGame>().AsSelf();
        builder.RegisterType<PokeCriesGame>().AsSelf();
        builder.RegisterType<GatekeepersGame>().AsSelf();

        builder.RegisterType<ElevenLabsAiTextToSpeechProvider>().As<IAiTextToSpeechProvider>().SingleInstance();
        builder.RegisterType<Gemini25FlashProvider>().AsSelf().SingleInstance();
        builder.RegisterType<MistralMediumProvider>().AsSelf().SingleInstance();
        builder.RegisterType<GptNano41Provider>().AsSelf().SingleInstance();
        builder.RegisterType<ConversationHistoryService>().As<IConversationHistoryService>().SingleInstance();
        builder.RegisterType<LanguageModelResolver>().As<ILanguageModelProvider>().SingleInstance();
        builder.RegisterType<ProfileService>().As<IProfileService>().SingleInstance();
        builder.RegisterType<ShowdownRanksProvider>().As<IShowdownRanksProvider>().SingleInstance();
        builder.RegisterType<LadderHistoryManager>().As<ILadderHistoryManager>().SingleInstance();
        builder.RegisterType<LadderTrackerManager>().As<ILadderTrackerManager>().SingleInstance();
        builder.RegisterType<PokepasteProvider>().As<ITeamProvider>().SingleInstance();
        builder.RegisterType<CoupCritiqueProvider>().As<ITeamProvider>().SingleInstance();
        builder.RegisterType<TeamLinkMatchFactory>().As<ITeamLinkMatchFactory>().SingleInstance();
        builder.RegisterType<DataManager>().As<IDataManager>().SingleInstance().OnActivating(e =>
        {
            e.Instance.Initialize().Wait(); // Risque d'ANR mais obligé pour garantir la bonne initialisation...
        });
        builder.RegisterType<ArcadeInscriptionsManager>().As<IArcadeInscriptionsManager>().SingleInstance();
        builder.RegisterType<UnsplashService>().As<IUnsplashService>().SingleInstance();
        builder.RegisterType<TenorService>().As<ITenorService>().SingleInstance();
    }

    private static void RegisterTournamentCommands(ContainerBuilder builder)
    {
        builder.RegisterCommand<SharedPowerCommand>();
        builder.RegisterCommand<TourHelpCommand>();
        builder.RegisterCommand<HebdoSvCommand>();
        builder.RegisterCommand<HebdoSsCommand>();
        builder.RegisterCommand<HebdoSmCommand>();
        builder.RegisterCommand<HebdoAaaCommand>();
        builder.RegisterCommand<HebdoBhCommand>();
        builder.RegisterCommand<HebdoMnMCommand>();
        builder.RegisterCommand<HebdoGgCommand>();
        builder.RegisterCommand<HebdoStabCommand>();
        builder.RegisterCommand<HebdoPiCCommand>();
        builder.RegisterCommand<HebdoInheCommand>();
        builder.RegisterCommand<HebdoCamoCommand>();
        builder.RegisterCommand<HebdoNfeCommand>();
        builder.RegisterCommand<Hebdo1V1Command>();
        builder.RegisterCommand<HebdoAgCommand>();
        builder.RegisterCommand<HebdoLcuuCommand>();
        builder.RegisterCommand<HebdoUbersUuCommand>();
        builder.RegisterCommand<HebdoZuCommand>();
        builder.RegisterCommand<HebdoAdvruCommand>();
        builder.RegisterCommand<HebdoBwruCommand>();
        builder.RegisterCommand<HebdoOrasruCommand>();
        builder.RegisterCommand<HebdoSmruCommand>();
        builder.RegisterCommand<HebdoSsruCommand>();
    }

    private static void RegisterShopCommands(ContainerBuilder builder)
    {
        builder.RegisterType<ShopService>().As<IShopService>().SingleInstance();
        builder.RegisterCommand<DisplayShopCommand>();
        builder.RegisterCommand<EditShopCommand>();
        builder.RegisterCommand<EditItemCommand>();
        builder.RegisterCommand<AddItemCommand>();
        builder.RegisterCommand<RemoveItemCommand>();
    }

    private static void RegisterRandomImagesCommands(ContainerBuilder builder)
    {
        builder.RegisterCommand<RandCatCommand>();
        builder.RegisterCommand<RandDogCommand>();
        builder.RegisterCommand<RandImageCommand>();
        builder.RegisterCommand<RandTurtleCommand>();
        builder.RegisterCommand<RandCapyCommand>();
        builder.RegisterCommand<RandGoatCommand>();
        builder.RegisterCommand<RandElephantCommand>();
        builder.RegisterCommand<RandPigCommand>();
        builder.RegisterCommand<RandBirdCommand>();
        builder.RegisterCommand<RandDolphinCommand>();
        builder.RegisterCommand<RandWolfCommand>();
        builder.RegisterCommand<RandTigerCommand>();
        builder.RegisterCommand<RandCheetahCommand>();
        builder.RegisterCommand<RandLionCommand>();
        builder.RegisterCommand<RandJaguarCommand>();
        builder.RegisterCommand<RandButterflyCommand>();
        builder.RegisterCommand<RandMouseCommand>();
        builder.RegisterCommand<RandMonkeyCommand>();
        builder.RegisterCommand<RandBearCommand>();
        builder.RegisterCommand<RandRabbitCommand>();
        builder.RegisterCommand<RandFrogCommand>();
        builder.RegisterCommand<RandSnakeCommand>();
        builder.RegisterCommand<RandSpiderCommand>();
        builder.RegisterCommand<RandSharkCommand>();
        builder.RegisterCommand<RandRacletteCommand>();
        builder.RegisterCommand<RandHeartGifCommand>();
        builder.RegisterCommand<RandCommand>();
        builder.RegisterCommand<RandGifCommand>();
        builder.RegisterCommand<RandMp4Command>();
        builder.RegisterCommand<RandFurretCommand>();
        builder.RegisterCommand<WalkCommand>();
        builder.RegisterCommand<RandHelpCommand>();
    }
}
