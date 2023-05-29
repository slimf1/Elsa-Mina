using Autofac;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Modules;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        
        /*
        builder.RegisterType<AddedCommandRepository>().As<IRepository<AddedCommand, Tuple<string, string>>>();
        builder.RegisterType<BadgeRepository>().As<IRepository<Badge, Tuple<string, string>>>();
        builder.RegisterType<RoomSpecificUserDataRepository>().As<IRepository<RoomSpecificUserData, Tuple<string, string>>>();
        builder.RegisterType<RoomParametersRepository>().As<IRepository<RoomParameters, string>>();*/
        
        builder.RegisterType<AddedCommandRepository>().AsSelf();
        builder.RegisterType<BadgeRepository>().AsSelf();
        builder.RegisterType<RoomSpecificUserDataRepository>().AsSelf();
        builder.RegisterType<RoomParametersRepository>().AsSelf();
        builder.RegisterType<BadgeHoldingRepository>().AsSelf();

        RegisterCachedRepository<AddedCommandRepository, AddedCommand, Tuple<string, string>>(builder);
        RegisterCachedRepository<BadgeRepository, Badge, Tuple<string, string>>(builder);
        RegisterCachedRepository<RoomSpecificUserDataRepository, RoomSpecificUserData, Tuple<string, string>>(builder);
        RegisterCachedRepository<RoomParametersRepository, RoomParameters, string>(builder);
        RegisterCachedRepository<BadgeHoldingRepository, BadgeHolding, Tuple<string, string, string>>(builder);
    }

    private void RegisterCachedRepository<TRepository, T, TKey>(ContainerBuilder builder)
        where T : IKeyed<TKey>
        where TRepository : IRepository<T, TKey>
    {
        builder.RegisterType<CachedRepository<TRepository, T, TKey>>().As<IRepository<T, TKey>>();
    }
}