using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randturtle")]
public class RandTurtleCommand : UnsplashRandomImageCommand
{
    protected override string Query => "turtle";

    public RandTurtleCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
