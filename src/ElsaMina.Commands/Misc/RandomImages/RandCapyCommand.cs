using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Images;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randcapy")]
public class RandCapyCommand : UnsplashRandomImageCommand
{
    protected override string Query => "capybara";

    public RandCapyCommand(IUnsplashService unsplashService, IImageService imageService) : base(unsplashService, imageService) { }
}
