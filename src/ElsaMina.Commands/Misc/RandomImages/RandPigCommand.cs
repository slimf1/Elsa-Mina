using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randpig")]
public class RandPigCommand : UnsplashRandomImageCommand
{
    protected override string Query => "pig";

    public RandPigCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
