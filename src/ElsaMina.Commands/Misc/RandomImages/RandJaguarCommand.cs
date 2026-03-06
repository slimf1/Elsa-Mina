using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randjaguar")]
public class RandJaguarCommand : UnsplashRandomImageCommand
{
    protected override string Query => "jaguar";

    public RandJaguarCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
