using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randtiger")]
public class RandTigerCommand : UnsplashRandomImageCommand
{
    protected override string Query => "tiger";

    public RandTigerCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
