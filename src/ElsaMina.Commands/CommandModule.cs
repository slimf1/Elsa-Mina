using System.Reflection;
using Autofac;
using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Commands.Ai.TextToSpeech;
using ElsaMina.Commands.ConnectFour;
using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Commands.GuessingGame.PokeCries;
using ElsaMina.Commands.GuessingGame.PokeDesc;
using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Commands.Teams.TeamProviders.CoupCritique;
using ElsaMina.Commands.Teams.TeamProviders.Pokepaste;
using ElsaMina.Core.Utils;
using Module = Autofac.Module;

namespace ElsaMina.Commands;

public class CommandModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterFromAssembly(Assembly.GetExecutingAssembly());

        builder.RegisterType<CountriesGame>().AsSelf();
        builder.RegisterType<ConnectFourGame>().AsSelf();
        builder.RegisterType<PokeDescGame>().AsSelf();
        builder.RegisterType<PokeCriesGame>().AsSelf();

        builder.RegisterType<ElevenLabsAiTextToSpeechProvider>().As<IAiTextToSpeechProvider>().SingleInstance();
        builder.RegisterType<MistralLanguageModelProvider>().As<ILanguageModelProvider>().SingleInstance();
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