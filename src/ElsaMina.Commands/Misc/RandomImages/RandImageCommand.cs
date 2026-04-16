using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randimage")]
public class RandImageCommand : UnsplashRandomImageCommand
{
    protected override string Query => "hearts";

    public RandImageCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
