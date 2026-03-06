using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randlion")]
public class RandLionCommand : UnsplashRandomImageCommand
{
    protected override string Query => "lion";

    public RandLionCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
