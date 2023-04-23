using Autofac;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Client;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Core.Modules;

public class MainModule : Module
{
    private static IContainer? _container;
    
    public static void Initialize()
    {
        var builder = new ContainerBuilder();

        builder.RegisterModule<MainModule>();
        builder.RegisterModule<CommandModule>();
        
        _container = builder.Build();
    }
    
    public static T Resolve<T>() where T : notnull
    {
        if (_container == null)
        {
            throw new Exception("Uninitialized DI container");
        }
        return _container.Resolve<T>();
    }

    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
        builder.RegisterType<HttpService>().As<IHttpService>().SingleInstance();
        builder.RegisterType<ClockService>().As<IClockService>().SingleInstance();

        builder.RegisterType<Client.Client>().As<IClient>().SingleInstance();
        builder.RegisterType<Bot.Bot>().As<IBot>().AsSelf().SingleInstance();
        
    }
}