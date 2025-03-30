using ElsaMina.Core.Models;
using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Config;

public class ConfigurationManager : IConfigurationManager
{
    public IConfiguration Configuration { get; private set; }

    public async Task LoadConfigurationAsync(TextReader textReader)
    {
        var json = await textReader.ReadToEndAsync();
        Configuration = JsonConvert.DeserializeObject<Configuration>(json);
        Log.Information("Loaded configuration");
    }
}