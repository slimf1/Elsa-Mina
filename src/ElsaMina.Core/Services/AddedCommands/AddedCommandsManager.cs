using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.AddedCommands;

public class AddedCommandsManager : IAddedCommandsManager
{
    private const int MAX_HEIGHT = 300;
    private const int MAX_WIDTH = 400;
    
    private readonly IAddedCommandRepository _addedCommandRepository;

    public AddedCommandsManager(IAddedCommandRepository addedCommandRepository)
    {
        _addedCommandRepository = addedCommandRepository;
    }

    public async Task TryExecuteAddedCommand(string commandName, IContext context)
    {
        var command =
            await _addedCommandRepository.GetByIdAsync(new Tuple<string, string>(commandName, context.RoomId));
        if (command == null)
        {
            return;
        }

        var content = command.Content;
        if (Images.IMAGE_LINK_REGEX.IsMatch(content))
        {
            var (width, height) = await Images.GetRemoteImageDimensions(content);
            (width, height) = Images.ResizeWithSameAspectRatio(width, height, MAX_WIDTH, MAX_HEIGHT);
            context.SendHtml($"""<img src="{content}" width="{width}" height="{height}" />""", rankAware: true);
            return;
        }
        
        // TODO : parsing w/ expressions
        context.Reply(content, rankAware: true);
    }
}