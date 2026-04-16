using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randfrog")]
public class RandFrogCommand : UnsplashRandomImageCommand
{
    protected override string Query => "frog";

    public RandFrogCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
