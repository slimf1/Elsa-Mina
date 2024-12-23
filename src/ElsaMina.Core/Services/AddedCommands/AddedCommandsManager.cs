using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Images;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.AddedCommands;

public class AddedCommandsManager : IAddedCommandsManager
{
    private const int MAX_HEIGHT = 300;
    private const int MAX_WIDTH = 400;
    
    private readonly IAddedCommandRepository _addedCommandRepository;
    private readonly IImageService _imageService;

    public AddedCommandsManager(IAddedCommandRepository addedCommandRepository,
        IImageService imageService)
    {
        _addedCommandRepository = addedCommandRepository;
        _imageService = imageService;
    }

    public async Task TryExecuteAddedCommand(string commandName, IContext context)
    {
        var command =
            await _addedCommandRepository.GetByIdAsync(Tuple.Create(commandName, context.RoomId));
        if (command == null)
        {
            return;
        }

        var content = command.Content;
        if (_imageService.IsLinkImage(content))
        {
            var (width, height) = await _imageService.GetRemoteImageDimensions(content);
            (width, height) = _imageService.ResizeWithSameAspectRatio(width, height, MAX_WIDTH, MAX_HEIGHT);
            context.SendHtml($"""<img src="{content}" width="{width}" height="{height}" />""", rankAware: true);
            return;
        }
        
        // TODO : parsing w/ expressions
        context.Reply(content, rankAware: true);
    }
}