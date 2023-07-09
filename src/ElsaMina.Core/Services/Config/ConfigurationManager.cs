using ElsaMina.Core.Models;
using Newtonsoft.Json;
using Serilog;

namespace ElsaMina.Core.Services.Config;

public class ConfigurationManager : IConfigurationManager
{
    private readonly ILogger _logger;

    public ConfigurationManager(ILogger logger)
    {
        _logger = logger;
    }

    public IConfiguration Configuration { get; private set; }

    public async Task LoadConfiguration(TextReader textReader)
    {
        var json = await textReader.ReadToEndAsync();
        Configuration = JsonConvert.DeserializeObject<Configuration>(json);
        _logger.Information("Loaded configuration: {0}", Configuration.Env);
    }
}