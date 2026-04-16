using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randmouse")]
public class RandMouseCommand : UnsplashRandomImageCommand
{
    protected override string Query => "mouse";
    protected override string WarningKey => "rand_warning_mouse";

    public RandMouseCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
