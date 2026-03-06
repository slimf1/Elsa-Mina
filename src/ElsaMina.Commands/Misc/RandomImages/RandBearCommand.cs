using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randbear")]
public class RandBearCommand : UnsplashRandomImageCommand
{
    protected override string Query => "bear";

    public RandBearCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
