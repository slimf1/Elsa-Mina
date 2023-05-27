using Autofac;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Modules;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<AddedCommandRepository>().AsSelf();
        builder.RegisterType<BadgeRepository>().AsSelf();
        builder.RegisterType<RoomSpecificUserDataRepository>().AsSelf();
        builder.RegisterType<RoomParametersRepository>().AsSelf();

        RegisterCachedRepository<AddedCommandRepository, AddedCommand, Tuple<string, string>>(builder);
        RegisterCachedRepository<BadgeRepository, Badge, Tuple<string, string>>(builder);
        RegisterCachedRepository<RoomSpecificUserDataRepository, RoomSpecificUserData, Tuple<string, string>>(builder);
        RegisterCachedRepository<RoomParametersRepository, RoomParameters, string>(builder);
    }

    private void RegisterCachedRepository<TRepository, T, TKey>(ContainerBuilder builder)
        where T : IKeyed<TKey>
        where TRepository : IRepository<T, TKey>
    {
        builder.RegisterType<CachedRepository<TRepository, T, TKey>>().As<IRepository<T, TKey>>();
    }
}