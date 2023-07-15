﻿using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;
using Serilog;

namespace ElsaMina.Core.Commands.Parsers;

public sealed class ChatMessageCommandParser : ChatMessageParser
{
    private readonly ILogger _logger;
    private readonly IRoomsManager _roomsManager;
    private readonly IConfigurationManager _configurationManager;
    private readonly ICommandExecutor _commandExecutor;
    
    public ChatMessageCommandParser(ILogger logger,
        IDependencyContainerService dependencyContainerService,
        IRoomsManager roomsManager,
        IConfigurationManager configurationManager,
        ICommandExecutor commandExecutor)
        : base(dependencyContainerService)
    {
        _logger = logger;
        _roomsManager = roomsManager;
        _configurationManager = configurationManager;
        _commandExecutor = commandExecutor;
    }
    
    protected override async Task HandleChatMessage(IContext context)
    {
        if (context.RoomId == null || !_roomsManager.HasRoom(context.RoomId))
        {
            return;
        }
        if (_configurationManager.Configuration.RoomBlacklist.Contains(context.RoomId))
        {
            return;
        }

        if (context.Command == null)
        {
            return;
        }
        try
        {
            await _commandExecutor.TryExecuteCommand(context.Command, context);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "Room Command execution crashed");
        }
    }
}