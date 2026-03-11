namespace ElsaMina.Commands.GuessingGame.Capitals;

public class CapitalCitiesGameData : ICapitalCitiesGameData
{
    public IReadOnlyList<CapitalCityData> Capitals { get; init; }
}
