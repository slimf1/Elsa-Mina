using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.GuessingGame.Countries;

public class CountriesGame : GuessingGame
{
    private readonly IRandomService _randomService;
    private readonly IDataManager _dataManager;

    public CountriesGame(ITemplatesManager templatesManager,
        IRandomService randomService,
        IConfiguration configuration,
        IDataManager dataManager,
        IClockService clockService) : base(templatesManager, configuration, clockService)
    {
        _randomService = randomService;
        _dataManager = dataManager;
    }

    public override string Identifier => nameof(CountriesGame);

    protected override void OnGameStart()
    {
        Context.ReplyLocalizedMessage("countries_game_start");
    }

    protected override Task OnTurnStart()
    {
        var nextCountry = _randomService.RandomElement(_dataManager.CountriesGameData.Countries);
        var image = _randomService.NextDouble() < 0.5
            ? nextCountry.Flag
            : nextCountry.Location;
        CurrentValidAnswers = [nextCountry.EnglishName, nextCountry.FrenchName];
        Context.Reply($"!show {image}");

        return Task.CompletedTask;
    }
}