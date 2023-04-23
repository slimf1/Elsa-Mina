using ElsaMina.Core.Bot;
using ElsaMina.Core.Modules;
using ElsaMina.Core.Services.Config;

var configurationFile = Environment.GetEnvironmentVariable("ELSA_MINA_ENV") switch
{
    "prod" => "prod.config.json",
    "dev" => "dev.config.json",
    _ => throw new Exception("Unknown environment")
};

MainModule.Initialize();
var configurationService = MainModule.Resolve<IConfigurationService>();
using var streamReader = new StreamReader(Path.Join("Config", configurationFile));
await configurationService.LoadConfiguration(streamReader);
var bot = MainModule.Resolve<IBot>();
bot.Start();