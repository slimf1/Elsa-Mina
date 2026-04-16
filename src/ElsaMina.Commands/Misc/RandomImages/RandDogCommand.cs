using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randdog")]
public class RandDogCommand : UnsplashRandomImageCommand
{
    protected override string Query => "dog";

    public RandDogCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
