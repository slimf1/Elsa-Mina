using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randsnake")]
public class RandSnakeCommand : UnsplashRandomImageCommand
{
    protected override string Query => "snake";
    protected override string WarningKey => "rand_warning_snake";

    public RandSnakeCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
