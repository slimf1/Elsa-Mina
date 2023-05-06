using ElsaMina.Core.Models;
using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Config;

public class ConfigurationService : IConfigurationService
{
    public IConfiguration Configuration { get; private set; }

    public async Task LoadConfiguration(TextReader textReader)
    {
        var json = await textReader.ReadToEndAsync();
        Configuration = JsonConvert.DeserializeObject<Configuration>(json);
    }
}