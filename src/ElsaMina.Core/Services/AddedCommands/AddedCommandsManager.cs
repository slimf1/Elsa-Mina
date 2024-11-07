using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Images;
using ElsaMina.Core.Utils;
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
            await _addedCommandRepository.GetByIdAsync(new Tuple<string, string>(commandName, context.RoomId));
        if (command == null)
        {
            return;
        }

        var content = command.Content;
        if (ImageService.IMAGE_LINK_REGEX.IsMatch(content))
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