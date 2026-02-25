namespace ElsaMina.Core.Services.Dex;

public interface IDexManager
{
    IReadOnlyDictionary<string, PokedexEntry> Pokedex { get; }
    IReadOnlyDictionary<string, MoveData> Moves { get; }
    Task LoadDexAsync(CancellationToken cancellationToken = default);
}