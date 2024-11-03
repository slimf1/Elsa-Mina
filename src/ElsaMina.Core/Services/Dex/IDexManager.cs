namespace ElsaMina.Core.Services.Dex;

public interface IDexManager
{
    IReadOnlyList<Pokemon> Pokedex { get; }
    Task LoadDex();
}