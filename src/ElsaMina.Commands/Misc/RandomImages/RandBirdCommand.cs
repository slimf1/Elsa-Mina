using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randbird")]
public class RandBirdCommand : UnsplashRandomImageCommand
{
    protected override string Query => "bird";

    public RandBirdCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
