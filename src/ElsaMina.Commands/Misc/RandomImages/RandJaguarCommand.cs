using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randjaguar")]
public class RandJaguarCommand : UnsplashRandomImageCommand
{
    protected override string Query => "jaguar";

    public RandJaguarCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
