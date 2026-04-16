using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randdolphin")]
public class RandDolphinCommand : UnsplashRandomImageCommand
{
    protected override string Query => "dolphin";

    public RandDolphinCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
