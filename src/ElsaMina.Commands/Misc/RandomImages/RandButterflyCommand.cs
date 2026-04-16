using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randbutterfly")]
public class RandButterflyCommand : UnsplashRandomImageCommand
{
    protected override string Query => "butterfly";

    public RandButterflyCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
