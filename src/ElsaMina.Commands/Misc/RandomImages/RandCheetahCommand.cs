using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randcheetah")]
public class RandCheetahCommand : UnsplashRandomImageCommand
{
    protected override string Query => "cheetah";

    public RandCheetahCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
