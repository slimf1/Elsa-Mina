namespace ElsaMina.Commands.Games.GuessingGame.Capitals;

public class CapitalCitiesGameData : ICapitalCitiesGameData
{
    public IReadOnlyList<CapitalCityData> Capitals { get; init; }
}
