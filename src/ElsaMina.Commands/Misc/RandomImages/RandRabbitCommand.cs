using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randrabbit")]
public class RandRabbitCommand : UnsplashRandomImageCommand
{
    protected override string Query => "rabbit";

    public RandRabbitCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
