﻿using Autofac;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Client;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Commands.Parsers;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.AddedCommands;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.Parsers;
using ElsaMina.Core.Services.PrivateMessages;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Services.UserDetails;
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
        builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>().SingleInstance();
        builder.RegisterType<HttpService>().As<IHttpService>().SingleInstance();
        builder.RegisterType<ClockService>().As<IClockService>().SingleInstance();
        builder.RegisterType<ContextFactory>().As<IContextFactory>().SingleInstance();
        builder.RegisterType<CommandExecutor>().As<ICommandExecutor>().SingleInstance();
        builder.RegisterType<RoomsManager>().As<IRoomsManager>().SingleInstance();
        builder.RegisterType<FormatsManager>().As<IFormatsManager>().SingleInstance();
        builder.RegisterType<LoginService>().As<ILoginService>().SingleInstance();
        builder.RegisterType<ResourcesService>().As<IResourcesService>().SingleInstance();
        builder.RegisterType<PmSendersManager>().As<IPmSendersManager>().SingleInstance();
        builder.RegisterType<ParsersManager>().As<IParsersManager>().SingleInstance();
        builder.RegisterType<AddedCommandsManager>().As<IAddedCommandsManager>().SingleInstance();
        builder.RegisterType<TemplatesManager>().As<ITemplatesManager>().SingleInstance();
        builder.RegisterType<RoomUserDataService>().As<IRoomUserDataService>();
        builder.RegisterType<UserDetailsManager>().As<IUserDetailsManager>().SingleInstance();
        builder.RegisterType<UserDataService>().As<IUserDataService>().SingleInstance();
        builder.RegisterType<RandomService>().As<IRandomService>().SingleInstance();

        builder.RegisterType<Client.Client>().As<IClient>().SingleInstance();
        builder.RegisterType<Bot.Bot>().As<IBot>().AsSelf().SingleInstance();
        
        builder.RegisterType<ChatMessageCommandParser>().As<IParser>().SingleInstance();
        builder.RegisterType<PrivateMessageCommandParser>().As<IParser>().SingleInstance();
    }
}