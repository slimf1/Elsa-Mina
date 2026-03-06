using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randdolphin")]
public class RandDolphinCommand : UnsplashRandomImageCommand
{
    protected override string Query => "dolphin";

    public RandDolphinCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
