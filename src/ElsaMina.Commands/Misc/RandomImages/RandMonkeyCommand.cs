using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randmonkey")]
public class RandMonkeyCommand : UnsplashRandomImageCommand
{
    protected override string Query => "monkey";

    public RandMonkeyCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
