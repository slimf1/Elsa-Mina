using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randtiger")]
public class RandTigerCommand : UnsplashRandomImageCommand
{
    protected override string Query => "tiger";

    public RandTigerCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
