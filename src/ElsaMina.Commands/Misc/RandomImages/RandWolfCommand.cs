using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randwolf")]
public class RandWolfCommand : UnsplashRandomImageCommand
{
    protected override string Query => "wolf";

    public RandWolfCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
