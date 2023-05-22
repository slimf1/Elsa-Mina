using System.Diagnostics;
using Autofac;
using ElsaMina.Commands;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Client;
using ElsaMina.Core.Modules;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using Serilog;

// DI 
var builder = new ContainerBuilder();
builder.RegisterModule<CoreModule>();
builder.RegisterModule<CommandModule>();
var container = builder.Build();
var dependencyContainerService = container.Resolve<IDependencyContainerService>();
dependencyContainerService.Container = container;
var logger = dependencyContainerService.Resolve<ILogger>();

// Load configuration
var configurationFile = Environment.GetEnvironmentVariable("ELSA_MINA_ENV") switch
{
    "prod" => "prod.config.json",
    "dev" => "dev.config.json",
    _ => throw new Exception("Unknown environment")
};
var configurationService = dependencyContainerService.Resolve<IConfigurationManager>();
using (var streamReader = new StreamReader(Path.Join("Config", configurationFile)))
{
    await configurationService.LoadConfiguration(streamReader);
}

// Subscribe to message event
var bot = dependencyContainerService.Resolve<IBot>();
var client = dependencyContainerService.Resolve<IClient>();
client.MessageReceived.Subscribe(message => Task.Run(async () => await bot.HandleReceivedMessage(message)));

// Disconnect event & reconnection logic
client.DisconnectionHappened.Subscribe(error =>
{
    logger.Error("Got disconnected : {0}\nrestarting in 30 seconds...", error);
    Thread.Sleep(30 * 1000);
    if (client.IsConnected)
    {
        logger.Error("Is still connected to server : Exiting");
        Environment.Exit(1);
        return;
    }
    logger.Information("Reconnecting...");
    Task.Run(client.Connect);
});

// Start
await bot.Start();
var exitEvent = new ManualResetEvent(false);
exitEvent.WaitOne();
