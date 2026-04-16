using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randshark")]
public class RandSharkCommand : UnsplashRandomImageCommand
{
    protected override string Query => "shark";
    protected override string WarningKey => "rand_warning_shark";

    public RandSharkCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
