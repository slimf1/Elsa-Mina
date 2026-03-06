using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randbutterfly")]
public class RandButterflyCommand : UnsplashRandomImageCommand
{
    protected override string Query => "butterfly";

    public RandButterflyCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
