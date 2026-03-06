using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randelephant")]
public class RandElephantCommand : UnsplashRandomImageCommand
{
    protected override string Query => "elephant";

    public RandElephantCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
