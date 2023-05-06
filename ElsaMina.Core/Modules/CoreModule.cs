using Autofac;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Client;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Rooms;
using Serilog;

namespace ElsaMina.Core.Modules;

public class CoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterModule<DataAccessModule>();

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day);
        builder.RegisterInstance(loggerConfig.CreateLogger()).As<ILogger>().SingleInstance();

        builder.RegisterType<DependencyContainerService>().As<IDependencyContainerService>().SingleInstance();
        builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
        builder.RegisterType<HttpService>().As<IHttpService>().SingleInstance();
        builder.RegisterType<ClockService>().As<IClockService>().SingleInstance();
        builder.RegisterType<ContextFactory>().As<IContextFactory>().SingleInstance();
        builder.RegisterType<CommandExecutor>().As<ICommandExecutor>().SingleInstance();
        builder.RegisterType<RoomsManager>().As<IRoomsManager>().SingleInstance();
        builder.RegisterType<FormatsManager>().As<IFormatsManager>().SingleInstance();

        builder.RegisterType<Client.Client>().As<IClient>().SingleInstance();
        builder.RegisterType<Bot.Bot>().As<IBot>().AsSelf().SingleInstance();
    }
}