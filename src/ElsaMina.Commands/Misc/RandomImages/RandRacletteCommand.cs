using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randraclette")]
public class RandRacletteCommand : UnsplashRandomImageCommand
{
    protected override string Query => "raclette";

    public RandRacletteCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
