using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.GuessingGame.PokeDesc;

public class PokeDescGame : GuessingGame
{
    private readonly IRandomService _randomService;
    private readonly IDataManager _dataManager;

    public PokeDescGame(ITemplatesManager templatesManager,
        IConfiguration configuration,
        IRandomService randomService,
        IDataManager dataManager,
        IClockService clockService) : base(templatesManager, configuration, clockService)
    {
        _randomService = randomService;
        _dataManager = dataManager;
    }

    public override string Identifier => nameof(PokeDescGame);

    protected override void OnGameStart()
    {
        Context.ReplyLocalizedMessage("pokedesc_start");
    }

    protected override Task OnTurnStart()
    {
        var randomDescription = _randomService.RandomElement(_dataManager.PokemonDescriptions);
        CurrentValidAnswers = [randomDescription.EnglishName, randomDescription.FrenchName];
        Context.Reply(randomDescription.Description);

        return Task.CompletedTask;
    }
}