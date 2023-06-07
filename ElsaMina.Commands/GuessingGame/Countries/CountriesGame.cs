using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templating;
using Newtonsoft.Json;

namespace ElsaMina.Commands.GuessingGame.Countries;

public class CountriesGame : GuessingGame
{
    private static readonly string GAME_FILE_PATH = Path.Join("Data", "countries_game.json");

    private readonly IRandomService _randomService;
    
    private readonly Lazy<CountriesGameData> _gameData = new(() =>
    {
        using var streamReader = new StreamReader(GAME_FILE_PATH);
        return JsonConvert.DeserializeObject<CountriesGameData>(streamReader.ReadToEnd());
    });

    public CountriesGame(ITemplatesManager templatesManager,
        IRandomService randomService,
        IConfigurationManager configurationManager) : base(templatesManager, configurationManager)
    {
        _randomService = randomService;
    }
    
    protected override void SendInitMessage()
    {
        Context.ReplyLocalizedMessage("countries_game_start");
    }

    protected override void SetupTurn()
    {
        var nextCountry = _randomService.RandomElement(_gameData.Value.Countries);
        var image = _randomService.NextDouble() < 0.5
            ? nextCountry.Flag
            : nextCountry.Location;
        CurrentValidAnswers = new[] { nextCountry.EnglishName, nextCountry.FrenchName };
        Context.Reply($"show {image}");
    }
}