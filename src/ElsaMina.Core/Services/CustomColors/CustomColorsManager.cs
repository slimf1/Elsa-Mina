using ElsaMina.Core.Services.Http;

namespace ElsaMina.Core.Services.CustomColors;

public class CustomColorsManager : ICustomColorsManager
{
    private const string CUSTOM_COLORS_FILE_URL = "https://play.pokemonshowdown.com/config/colors.json";
    
    private readonly IHttpService _httpService;

    public CustomColorsManager(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public IReadOnlyDictionary<string, string> CustomColorsMapping { get; private set; }
        = new Dictionary<string, string>();
    
    public async Task FetchCustomColors()
    {
        try
        {
           CustomColorsMapping = await _httpService.Get<Dictionary<string, string>>(CUSTOM_COLORS_FILE_URL);
           Logger.Current.Information("Fetched {0} custom colors", CustomColorsMapping.Count);
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "Could not fetch custom colors");
        }
    }
}