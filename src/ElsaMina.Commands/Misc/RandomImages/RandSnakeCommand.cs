using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randsnake")]
public class RandSnakeCommand : UnsplashRandomImageCommand
{
    protected override string Query => "snake";
    protected override string WarningKey => "rand_warning_snake";

    public RandSnakeCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
