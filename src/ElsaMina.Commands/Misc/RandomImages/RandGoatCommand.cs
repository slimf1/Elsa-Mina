using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randgoat")]
public class RandGoatCommand : UnsplashRandomImageCommand
{
    protected override string Query => "goat";

    public RandGoatCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
