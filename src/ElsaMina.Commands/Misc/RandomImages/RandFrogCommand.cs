using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randfrog")]
public class RandFrogCommand : UnsplashRandomImageCommand
{
    protected override string Query => "frog";

    public RandFrogCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
