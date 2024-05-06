using ElsaMina.Core;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;
using Newtonsoft.Json;

namespace ElsaMina.Commands.GuessingGame.Countries;

public class CountriesGame : GuessingGame
{
    private static readonly string GAME_FILE_PATH = Path.Join("Data", "countries_game.json");
    private static CountriesGameData CountriesGameData { get; set; }

    private readonly IRandomService _randomService;

    public static async Task LoadCountriesGameData()
    {
        using var streamReader = new StreamReader(GAME_FILE_PATH);
        var fileContent = await streamReader.ReadToEndAsync();
        CountriesGameData = JsonConvert.DeserializeObject<CountriesGameData>(fileContent);
        Logger.Current.Debug("Loaded countries game data with {0} entries", CountriesGameData.Countries.Count());
    }

    public CountriesGame(ITemplatesManager templatesManager,
        IRandomService randomService,
        IConfigurationManager configurationManager) : base(templatesManager, configurationManager)
    {
        _randomService = randomService;
    }

    protected override void OnGameStart()
    {
        Context.ReplyLocalizedMessage("countries_game_start");
    }

    protected override void OnTurnStart()
    {
        var nextCountry = _randomService.RandomElement(CountriesGameData.Countries);
        var image = _randomService.NextDouble() < 0.5
            ? nextCountry.Flag
            : nextCountry.Location;
        CurrentValidAnswers = new[] { nextCountry.EnglishName, nextCountry.FrenchName };
        Context.Reply($"show {image}");
    }
}