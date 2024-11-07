using ElsaMina.Core;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Images;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using Newtonsoft.Json;

namespace ElsaMina.Commands.GuessingGame.Countries;

public class CountriesGame : GuessingGame
{
    private const int MAX_HEIGHT = 200;
    private const int MAX_WIDTH = 300;
    
    private static readonly string GAME_FILE_PATH = Path.Join("Data", "countries_game.json");
    private static CountriesGameData CountriesGameData { get; set; }

    private readonly IRandomService _randomService;
    private readonly IImageService _imageService;

    public static async Task LoadCountriesGameData()
    {
        using var streamReader = new StreamReader(GAME_FILE_PATH);
        var fileContent = await streamReader.ReadToEndAsync();
        CountriesGameData = JsonConvert.DeserializeObject<CountriesGameData>(fileContent);
        Logger.Information("Loaded countries game data with {0} entries", CountriesGameData.Countries.Count());
    }

    public CountriesGame(ITemplatesManager templatesManager,
        IRandomService randomService,
        IConfigurationManager configurationManager,
        IImageService imageService) : base(templatesManager, configurationManager)
    {
        _randomService = randomService;
        _imageService = imageService;
    }

    public override string Identifier => nameof(CountriesGame);

    protected override void OnGameStart()
    {
        Context.ReplyLocalizedMessage("countries_game_start");
    }

    protected override async Task OnTurnStart()
    {
        var nextCountry = _randomService.RandomElement(CountriesGameData.Countries);
        var image = _randomService.NextDouble() < 0.5
            ? nextCountry.Flag
            : nextCountry.Location;
        CurrentValidAnswers = [nextCountry.EnglishName, nextCountry.FrenchName];
        var (width, height) = await _imageService.GetRemoteImageDimensions(image);
        (width, height) = _imageService.ResizeWithSameAspectRatio(width, height, MAX_WIDTH, MAX_HEIGHT);
        Context.SendHtml($"""<img src="{image}" width="{width}" height="{height}" /> """);
    }
}