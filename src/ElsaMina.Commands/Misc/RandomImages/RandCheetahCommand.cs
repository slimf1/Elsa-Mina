using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randcheetah")]
public class RandCheetahCommand : UnsplashRandomImageCommand
{
    protected override string Query => "cheetah";

    public RandCheetahCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
