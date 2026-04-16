using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randlion")]
public class RandLionCommand : UnsplashRandomImageCommand
{
    protected override string Query => "lion";

    public RandLionCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
