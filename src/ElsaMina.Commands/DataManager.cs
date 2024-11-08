using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Commands.GuessingGame.PokeDesc;
using ElsaMina.Core;
using Newtonsoft.Json;

namespace ElsaMina.Commands;

public class DataManager : IDataManager
{
    public CountriesGameData CountriesGameData { get; private set; }
    public IReadOnlyList<PokemonDescription> PokemonDescriptions { get; private set; }

    public async Task Initialize()
    {
        CountriesGameData = await GetDataFromFile<CountriesGameData>("./Data/countries_game.json");
        PokemonDescriptions = await GetDataFromFile<List<PokemonDescription>>("./Data/pokedesc.json");

        Logger.Information("Fetched countries & pokemon descriptions.");
    }

    private static async Task<T> GetDataFromFile<T>(string filePath)
    {
        using var streamReader = new StreamReader(filePath);
        var fileContent = await streamReader.ReadToEndAsync();
        return JsonConvert.DeserializeObject<T>(fileContent);
    }
}