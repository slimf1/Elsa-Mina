using ElsaMina.Core.Bot;
using ElsaMina.Core.Client;
using ElsaMina.Core.Modules;
using ElsaMina.Core.Services.Config;

var configurationFile = Environment.GetEnvironmentVariable("ELSA_MINA_ENV") switch
{
    "prod" => "prod.config.json",
    "dev" => "dev.config.json",
    _ => throw new Exception("Unknown environment")
};

MainModule.Initialize();

// Load configuration
var configurationService = MainModule.Resolve<IConfigurationService>();
using (var streamReader = new StreamReader(Path.Join("Config", configurationFile)))
{
    await configurationService.LoadConfiguration(streamReader);
}

// Subscribe to message event
var bot = MainModule.Resolve<IBot>();
var client = MainModule.Resolve<IClient>();
client.MessageReceived.Subscribe(message => Task.Run(async () => await bot.HandleReceivedMessage(message)));

// Start
await bot.Start();
var exitEvent = new ManualResetEvent(false);
exitEvent.WaitOne();
