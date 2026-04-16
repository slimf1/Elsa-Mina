using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randspider")]
public class RandSpiderCommand : UnsplashRandomImageCommand
{
    protected override string Query => "spider";
    protected override string WarningKey => "rand_warning_spider";

    public RandSpiderCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
