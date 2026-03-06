using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randbird")]
public class RandBirdCommand : UnsplashRandomImageCommand
{
    protected override string Query => "bird";

    public RandBirdCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
