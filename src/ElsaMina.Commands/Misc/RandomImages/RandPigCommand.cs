using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randpig")]
public class RandPigCommand : UnsplashRandomImageCommand
{
    protected override string Query => "pig";

    public RandPigCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
