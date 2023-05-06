using Autofac;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Modules;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<AddedCommandRepository>().As<IAddedCommandRepository>().SingleInstance();
        builder.RegisterType<BadgeRepository>().As<IBadgeRepository>().SingleInstance();
        builder.RegisterType<RoomSpecificUserDataRepository>().As<IRoomSpecificUserDataRepository>().SingleInstance();
    }
}