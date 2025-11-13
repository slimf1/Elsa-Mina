using Autofac;
using ElsaMina.Core.Services.Config;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Core.Modules;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<BotDbContext>().As<DbContext>().OnActivating(e =>
        {
            var configuration = e.Context.Resolve<IConfiguration>();
            e.Instance.MaxRetries = configuration.DatabaseMaxRetries;
            e.Instance.ConnectionString = configuration.ConnectionString;
            e.Instance.RetryDelay = configuration.DatabaseRetryDelay;
        });
        builder.RegisterType<AddedCommandRepository>().As<IAddedCommandRepository>();
        builder.RegisterType<BadgeRepository>().As<IBadgeRepository>();
        builder.RegisterType<RoomSpecificUserDataRepository>().As<IRoomSpecificUserDataRepository>();
        builder.RegisterType<RoomInfoRepository>().As<IRoomInfoRepository>();
        builder.RegisterType<BadgeHoldingRepository>().As<IBadgeHoldingRepository>();
        builder.RegisterType<TeamRepository>().As<ITeamRepository>();
        builder.RegisterType<RoomBotParameterValueRepository>().As<IRoomBotParameterValueRepository>();
        builder.RegisterType<ArcadeLevelRepository>().As<IArcadeLevelRepository>();
        builder.RegisterType<PollSuggestionRepository>().As<IPollSuggestionRepository>();
        builder.RegisterType<UserPlayTimeRepository>().As<IUserPlayTimeRepository>();
        builder.RegisterType<SavedPollRepository>().As<ISavedPollRepository>();
    }
}