using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.AddedCommands;

public class AddedCommandsManager : IAddedCommandsManager
{
    private readonly IRepository<AddedCommand, Tuple<string, string>> _addedCommandRepository;

    public AddedCommandsManager(IRepository<AddedCommand, Tuple<string, string>> addedCommandRepository)
    {
        _addedCommandRepository = addedCommandRepository;
    }

    public async Task TryExecuteAddedCommand(string commandName, IContext context)
    {
        if (!context.HasSufficientRank('+') && !context.IsSenderWhitelisted) // TODO : Parameterize via room parameters ?
        {
            return;
        }

        var command = await _addedCommandRepository.GetByIdAsync(new(commandName, context.RoomId));
        if (command == null)
        {
            return;
        }
        
        context.Reply(GetMessageFromCommand(command));
    }
    
    private string GetMessageFromCommand(AddedCommand command)
    {
        // TODO : parsing
        return command.Content;
    }
}