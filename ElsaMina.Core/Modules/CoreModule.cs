using Autofac;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Client;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Core.Modules;

public class CoreModule : Module
{
    // Virer les statics => services singleton
    public static IContainer? Container { get; set; }

    public static T Resolve<T>() where T : notnull
    {
        if (Container == null)
        {
            throw new Exception("Uninitialized DI container");
        }
        return Container.Resolve<T>();
    }

    public static ICommand ResolveCommand(string commandName)
    {
        if (Container == null)
        {
            throw new Exception("Uninitialized DI container");
        }
        return Container.ResolveNamed<ICommand>(commandName);
    }
    
    public static bool IsCommandRegistered(string commandName)
    {
        if (Container == null)
        {
            throw new Exception("Uninitialized DI container");
        }
        return Container.IsRegisteredWithName<ICommand>(commandName);
    }

    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
        builder.RegisterType<HttpService>().As<IHttpService>().SingleInstance();
        builder.RegisterType<ClockService>().As<IClockService>().SingleInstance();
        builder.RegisterType<ContextFactory>().As<IContextFactory>().SingleInstance();
        builder.RegisterType<CommandExecutor>().As<ICommandExecutor>().SingleInstance();

        builder.RegisterType<Client.Client>().As<IClient>().SingleInstance();
        builder.RegisterType<Bot.Bot>().As<IBot>().AsSelf().SingleInstance();
        
    }
}