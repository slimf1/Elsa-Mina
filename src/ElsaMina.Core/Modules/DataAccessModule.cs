﻿using Autofac;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Modules;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        
        builder.RegisterType<AddedCommandRepository>().As<IAddedCommandRepository>();
        builder.RegisterType<BadgeRepository>().As<IBadgeRepository>();
        builder.RegisterType<RoomSpecificUserDataRepository>().As<IRoomSpecificUserDataRepository>();
        builder.RegisterType<RoomParametersRepository>().As<IRoomParametersRepository>();
        builder.RegisterType<BadgeHoldingRepository>().As<IBadgeHoldingRepository>();
        builder.RegisterType<TeamRepository>().As<ITeamRepository>();
        builder.RegisterType<RoomBotParameterValueRepository>().As<IRoomBotParameterValueRepository>();
    }
}