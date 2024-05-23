using Autofac;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.AddedCommands;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.PrivateMessages;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Repeats;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Services.System;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Services.UserDetails;

namespace ElsaMina.Core.Modules;

public class CoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterModule<DataAccessModule>();

        builder.RegisterType<DependencyContainerService>().As<IDependencyContainerService>().SingleInstance();
        builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>().SingleInstance();
        builder.RegisterType<HttpService>().As<IHttpService>().SingleInstance();
        builder.RegisterType<ClockService>().As<IClockService>().SingleInstance();
        builder.RegisterType<ContextFactory>().As<IContextFactory>().SingleInstance();
        builder.RegisterType<DefaultContextProvider>().As<IContextProvider>().SingleInstance();
        builder.RegisterType<CommandExecutor>().As<ICommandExecutor>().SingleInstance();
        builder.RegisterType<RoomsManager>().As<IRoomsManager>().SingleInstance();
        builder.RegisterType<FormatsManager>().As<IFormatsManager>().SingleInstance();
        builder.RegisterType<LoginService>().As<ILoginService>().SingleInstance();
        builder.RegisterType<ResourcesService>().As<IResourcesService>().SingleInstance();
        builder.RegisterType<PmSendersManager>().As<IPmSendersManager>().SingleInstance();
        builder.RegisterType<HandlerManager>().As<IHandlerManager>().SingleInstance();
        builder.RegisterType<AddedCommandsManager>().As<IAddedCommandsManager>().SingleInstance();
        builder.RegisterType<TemplatesManager>().As<ITemplatesManager>().SingleInstance();
        builder.RegisterType<RoomUserDataService>().As<IRoomUserDataService>();
        builder.RegisterType<UserDetailsManager>().As<IUserDetailsManager>().SingleInstance();
        builder.RegisterType<UserDataService>().As<IUserDataService>().SingleInstance();
        builder.RegisterType<RandomService>().As<IRandomService>().SingleInstance();
        builder.RegisterType<RepeatsManager>().As<IRepeatsManager>().SingleInstance();
        builder.RegisterType<SystemService>().As<ISystemService>().SingleInstance();
        builder.RegisterType<RoomConfigurationParametersFactory>().As<IRoomConfigurationParametersFactory>()
            .SingleInstance();

        builder.RegisterType<Client>().As<IClient>().SingleInstance();
        builder.RegisterType<Bot>().As<IBot>().AsSelf().SingleInstance();
        
        builder.RegisterType<ChatMessageCommandHandler>().As<IHandler>().SingleInstance();
        builder.RegisterType<PrivateMessageCommandHandler>().As<IHandler>().SingleInstance();
        builder.RegisterType<NameTakenHandler>().As<IHandler>().SingleInstance();
        builder.RegisterType<QueryResponseHandler>().As<IHandler>().SingleInstance();
        builder.RegisterType<RoomsHandler>().As<IHandler>().SingleInstance();
    }
}