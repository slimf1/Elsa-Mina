using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randgoat")]
public class RandGoatCommand : UnsplashRandomImageCommand
{
    protected override string Query => "goat";

    public RandGoatCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
