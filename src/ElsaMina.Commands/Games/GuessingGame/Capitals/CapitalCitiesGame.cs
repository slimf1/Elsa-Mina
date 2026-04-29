using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Games.GuessingGame.Capitals;

public class CapitalCitiesGame : GuessingGame
{
    private readonly IRandomService _randomService;
    private readonly IDataManager _dataManager;

    public CapitalCitiesGame(ITemplatesManager templatesManager,
        IRandomService randomService,
        IConfiguration configuration,
        IDataManager dataManager,
        IClockService clockService) : base(templatesManager, configuration, clockService)
    {
        _randomService = randomService;
        _dataManager = dataManager;
    }

    public override string Identifier => nameof(CapitalCitiesGame);

    protected override void OnGameStart()
    {
        Context.ReplyLocalizedMessage("capitals_game_start");
    }

    protected override Task OnTurnStart()
    {
        var entry = _randomService.RandomElement(_dataManager.CapitalCitiesGameData.Capitals);
        var isFrench = Context.Culture.TwoLetterISOLanguageName == "fr";

        if (_randomService.NextDouble() < 0.5)
        {
            CurrentValidAnswers = [entry.CapitalEnglish, entry.CapitalFrench];
            Context.ReplyLocalizedMessage("capitals_game_question_country",
                isFrench ? entry.CountryFrench : entry.CountryEnglish);
        }
        else
        {
            CurrentValidAnswers = [entry.CountryEnglish, entry.CountryFrench];
            Context.ReplyLocalizedMessage("capitals_game_question_capital",
                isFrench ? entry.CapitalFrench : entry.CapitalEnglish);
        }

        return Task.CompletedTask;
    }
}
