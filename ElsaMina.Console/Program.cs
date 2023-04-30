using Autofac;
using ElsaMina.Commands;
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

// DI 
var builder = new ContainerBuilder();
builder.RegisterModule<CoreModule>();
builder.RegisterModule<CommandModule>();
CoreModule.Container = builder.Build();

// Load configuration
var configurationService = CoreModule.Resolve<IConfigurationService>();
using (var streamReader = new StreamReader(Path.Join("Config", configurationFile)))
{
    await configurationService.LoadConfiguration(streamReader);
}

// Subscribe to message event
var bot = CoreModule.Resolve<IBot>();
var client = CoreModule.Resolve<IClient>();
client.MessageReceived.Subscribe(message => Task.Run(async () => await bot.HandleReceivedMessage(message)));

// Start
await bot.Start();
var exitEvent = new ManualResetEvent(false);
exitEvent.WaitOne();
