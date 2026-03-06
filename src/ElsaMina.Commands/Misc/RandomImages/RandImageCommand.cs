using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randimage")]
public class RandImageCommand : UnsplashRandomImageCommand
{
    protected override string Query => "hearts";

    public RandImageCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
