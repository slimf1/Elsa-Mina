namespace ElsaMina.Commands.GuessingGame.Capitals;

public interface ICapitalCitiesGameData
{
    IReadOnlyList<CapitalCityData> Capitals { get; }
}
