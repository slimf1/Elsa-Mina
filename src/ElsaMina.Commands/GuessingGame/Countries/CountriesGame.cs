using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Images;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.GuessingGame.Countries;

public class CountriesGame : GuessingGame
{
    private const int MAX_HEIGHT = 200;
    private const int MAX_WIDTH = 300;

    private readonly IRandomService _randomService;
    private readonly IImageService _imageService;
    private readonly IDataManager _dataManager;

    public CountriesGame(ITemplatesManager templatesManager,
        IRandomService randomService,
        IConfiguration configuration,
        IImageService imageService,
        IDataManager dataManager,
        IClockService clockService) : base(templatesManager, configuration, clockService)
    {
        _randomService = randomService;
        _imageService = imageService;
        _dataManager = dataManager;
    }

    public override string Identifier => nameof(CountriesGame);

    protected override void OnGameStart()
    {
        Context.ReplyLocalizedMessage("countries_game_start");
    }

    protected override async Task OnTurnStart()
    {
        var nextCountry = _randomService.RandomElement(_dataManager.CountriesGameData.Countries);
        var image = _randomService.NextDouble() < 0.5
            ? nextCountry.Flag
            : nextCountry.Location;
        CurrentValidAnswers = [nextCountry.EnglishName, nextCountry.FrenchName];
        var (width, height) = await _imageService.GetRemoteImageDimensions(image);

        if (width <= 0 || height <= 0)
        {
            // Failsafe in case we get rate limited or IP banned by imgur
            // but this breaks the aspect ratio of the image
            width = MAX_WIDTH;
            height = MAX_HEIGHT;
        }
        else
        {
            (width, height) = ImageUtils.ResizeWithSameAspectRatio(width, height, MAX_WIDTH, MAX_HEIGHT);
        }

        Context.ReplyHtml($"""<img src="{image}" width="{width}" height="{height}" />""");
    }
}