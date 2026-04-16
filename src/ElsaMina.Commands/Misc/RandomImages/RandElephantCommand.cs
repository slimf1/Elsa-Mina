using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randelephant")]
public class RandElephantCommand : UnsplashRandomImageCommand
{
    protected override string Query => "elephant";

    public RandElephantCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
