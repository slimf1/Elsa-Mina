namespace ElsaMina.Core.Services.CustomColors;

public interface ICustomColorsManager
{
    IReadOnlyDictionary<string, string> CustomColorsMapping { get; }

    Task FetchCustomColorsAsync();
}