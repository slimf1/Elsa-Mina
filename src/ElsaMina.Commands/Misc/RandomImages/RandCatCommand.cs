using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randcat")]
public class RandCatCommand : UnsplashRandomImageCommand
{
    protected override string Query => "cat";

    public RandCatCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
