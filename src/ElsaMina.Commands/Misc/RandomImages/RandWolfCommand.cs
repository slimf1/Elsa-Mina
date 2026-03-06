using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randwolf")]
public class RandWolfCommand : UnsplashRandomImageCommand
{
    protected override string Query => "wolf";

    public RandWolfCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
