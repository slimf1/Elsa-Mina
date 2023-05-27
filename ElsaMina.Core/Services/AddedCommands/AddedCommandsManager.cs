using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using Serilog;

namespace ElsaMina.Core.Services.AddedCommands;

public class AddedCommandsManager : IAddedCommandsManager
{
    private readonly Dictionary<Tuple<string, string>, AddedCommand> _addedCommandsCache = new();

    private readonly ILogger _logger;
    private readonly IAddedCommandRepository _addedCommandRepository;

    public AddedCommandsManager(ILogger logger, IAddedCommandRepository addedCommandRepository)
    {
        _logger = logger;
        _addedCommandRepository = addedCommandRepository;
    }

    public async Task TryExecuteAddedCommand(string commandName, IContext context)
    {
        if (!context.HasSufficientRank('+') && !context.IsSenderWhitelisted) // TODO : Parameterize via room parameters ?
        {
            return;
        }

        var command = await GetCommand(commandName, context.RoomId);
        if (command == null)
        {
            return;
        }
        
        context.Reply(GetMessageFromCommand(command));
    }
    
    private async Task<AddedCommand> GetCommand(string commandId, string roomId)
    {
        var key = new Tuple<string, string>(commandId, roomId);
        if (_addedCommandsCache.TryGetValue(key, out var cachedCommand))
        {
            return cachedCommand;
        }

        try
        {
            var command = await _addedCommandRepository.GetByIdAsync(commandId, roomId);
            if (command != null)
            {
                _addedCommandsCache[key] = command;
            }

            return command;
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occured while fetching a command");
            return null;
        }
    }

    private string GetMessageFromCommand(AddedCommand command)
    {
        // TODO : parsing
        return command.Content;
    }
}