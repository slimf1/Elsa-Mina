using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randturtle")]
public class RandTurtleCommand : UnsplashRandomImageCommand
{
    protected override string Query => "turtle";

    public RandTurtleCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
