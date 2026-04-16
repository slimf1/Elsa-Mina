using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randmonkey")]
public class RandMonkeyCommand : UnsplashRandomImageCommand
{
    protected override string Query => "monkey";

    public RandMonkeyCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
