using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randbear")]
public class RandBearCommand : UnsplashRandomImageCommand
{
    protected override string Query => "bear";

    public RandBearCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
