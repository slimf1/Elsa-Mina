namespace ElsaMina.Commands.Games.GuessingGame.Capitals;

public interface ICapitalCitiesGameData
{
    IReadOnlyList<CapitalCityData> Capitals { get; }
}
