using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Dex;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.GuessingGame.Gatekeepers;

public class GatekeepersGame : GuessingGame
{
    private const int ANSWERS_COUNT = 4;
    private const string TEMPLATE_PATH = "GuessingGame/Gatekeepers/GatekeepersGamePanel";
    private const string FOOTPRINT_SPRITE_URL =
        "https://raw.githubusercontent.com/slimf1/Elsa-Mina/refs/heads/master/src/ElsaMina.Commands/Data/Footprints/{0}.png";
    private const int SHOW_PORTRAITS_AT = 10;
    private const int SHOW_SILHOUETTES_AT = 5;
    
    private static int NextGameId { get; set; } = 1;
    
    private readonly IDexManager _dexManager;
    private readonly IRandomService _randomService;
    private readonly ITemplatesManager _templatesManager;
    private readonly int _gameId;
    private readonly List<Pokemon> _currentOptions = [];
    private Pokemon _currentValidAnswer;
    
    public GatekeepersGame(ITemplatesManager templatesManager, IConfigurationManager configurationManager,
        IDexManager dexManager, IRandomService randomService)
        : base(templatesManager, configurationManager)
    {
        _templatesManager = templatesManager;
        _dexManager = dexManager;
        _randomService = randomService;
        
        _gameId = NextGameId++;
    }

    public override string Identifier => nameof(GatekeepersGame);

    private string HtmlId => $"gatekeepers-{_gameId}-t{CurrentTurn}";

    protected override void OnGameStart()
    {
    }

    protected override async Task OnTurnStart()
    {
        var slice = _dexManager.Pokedex.Take(649);
        var sample = _randomService.RandomSample(slice, ANSWERS_COUNT).ToList();

        _currentOptions.Clear();
        _currentOptions.AddRange(sample);
        _currentValidAnswer = _randomService.RandomElement(_currentOptions);
        CurrentValidAnswers = [_currentValidAnswer.Name.English];

        var viewModel = new GatekeepersGamePanelViewModel
        {
            Culture = Context.Culture,
            FootprintSprite = string.Format(FOOTPRINT_SPRITE_URL, _currentValidAnswer.PokedexId),
            ShowPortraits = false,
            ShowSilhouettes = false,
            Scores = Scores
        };

        var template = await _templatesManager.GetTemplateAsync(TEMPLATE_PATH, viewModel);
        Context.SendUpdatableHtml(HtmlId, template.RemoveNewlines(), isChanging: false);
    }

    protected override void OnTimerCountdown(TimeSpan remainingTime)
    {
        base.OnTimerCountdown(remainingTime);
        
        if (remainingTime.TotalSeconds.IsApproximatelyEqualTo(SHOW_PORTRAITS_AT))
        {
            var viewModel = new GatekeepersGamePanelViewModel
            {
                Culture = Context.Culture,
                FootprintSprite = string.Format(FOOTPRINT_SPRITE_URL, _currentValidAnswer.PokedexId),
                ShowPortraits = true,
                ShowSilhouettes = false,
                Scores = Scores
            };
            var template = _templatesManager.GetTemplateAsync(TEMPLATE_PATH, viewModel).Result;
            Context.SendUpdatableHtml(HtmlId, template.RemoveNewlines(), isChanging: true);
            
        }
        else if (remainingTime.TotalSeconds.IsApproximatelyEqualTo(SHOW_SILHOUETTES_AT))
        {
            var viewModel = new GatekeepersGamePanelViewModel
            {
                Culture = Context.Culture,
                FootprintSprite = string.Format(FOOTPRINT_SPRITE_URL, _currentValidAnswer.PokedexId),
                ShowPortraits = true,
                ShowSilhouettes = true,
                Scores = Scores
            };
            var template = _templatesManager.GetTemplateAsync(TEMPLATE_PATH, viewModel).Result;
            Context.SendUpdatableHtml(HtmlId, template.RemoveNewlines(), isChanging: true);
        }
        
    }
}