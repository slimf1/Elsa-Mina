using ElsaMina.Core;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;
using Newtonsoft.Json;

namespace ElsaMina.Commands.GuessingGame.PokeDesc;

public class PokeDescGame : GuessingGame
{
    private static readonly string GAME_FILE_PATH = Path.Join("Data", "pokedesc.json");
    private static List<PokemonDescription> PokeDescGameData { get; set; }

    public static async Task LoadPokeDescData()
    {
        using var streamReader = new StreamReader(GAME_FILE_PATH);
        var fileContent = await streamReader.ReadToEndAsync();
        PokeDescGameData = JsonConvert.DeserializeObject<List<PokemonDescription>>(fileContent);
        Logger.Information("Loaded PokeDesc game data with {0} entries", PokeDescGameData.Count);
    }

    private readonly IRandomService _randomService;

    public PokeDescGame(ITemplatesManager templatesManager,
        IConfigurationManager configurationManager,
        IRandomService randomService) : base(templatesManager, configurationManager)
    {
        _randomService = randomService;
    }

    public override string Identifier => nameof(PokeDescGame);

    protected override void OnGameStart()
    {
        base.OnGameStart();
        Context.ReplyLocalizedMessage("pokedesc_start");
    }

    protected override Task OnTurnStart()
    {
        var randomDescription = _randomService.RandomElement(PokeDescGameData);
        CurrentValidAnswers = [randomDescription.EnglishName, randomDescription.FrenchName];
        Context.Reply(randomDescription.Description);

        return Task.CompletedTask;
    }
}