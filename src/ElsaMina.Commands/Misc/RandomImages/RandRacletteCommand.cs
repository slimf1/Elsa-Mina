using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randraclette")]
public class RandRacletteCommand : UnsplashRandomImageCommand
{
    protected override string Query => "raclette";

    public RandRacletteCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
