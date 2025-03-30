using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Autofac;
using ElsaMina.Commands;
using ElsaMina.Console;
using ElsaMina.Core;
using ElsaMina.Core.Modules;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;

// DI 
var builder = new ContainerBuilder();
builder.RegisterModule<CoreModule>();
builder.RegisterModule<CommandModule>();
builder.RegisterType<VersionProvider>().As<IVersionProvider>();
var container = builder.Build();
var dependencyContainerService = container.Resolve<IDependencyContainerService>();
dependencyContainerService.SetContainer(container);
DependencyContainerService.Current = dependencyContainerService;

// Load configuration file
var configurationService = dependencyContainerService.Resolve<IConfigurationManager>();
using (var streamReader = new StreamReader("config.json"))
{
    await configurationService.LoadConfigurationAsync(streamReader);
}

// Subscribe to message event
var bot = dependencyContainerService.Resolve<IBot>();
var client = dependencyContainerService.Resolve<IClient>();
client.MessageReceived
    .Select(message => bot.HandleReceivedMessage(message).ToObservable())
    .Concat()
    .Catch((Exception exception) =>
    {
        Log.Error(exception, "Error while handling message");
        return Observable.Throw<Unit>(exception);
    })
    .Subscribe();

// Disconnect event
client.DisconnectionHappened.Subscribe(error =>
{
    Log.Error("Got disconnected : {0}", error);
    bot.OnDisconnect();
});

// Reconnection
client.ReconnectionHappened.Subscribe(info =>
{
    Log.Information("Reconnecting : {0}", info.Type);
    bot.OnReconnect();
});

// Start
await bot.Start();
var exitEvent = new ManualResetEvent(false);
exitEvent.WaitOne();
