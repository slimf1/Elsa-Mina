using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randdog")]
public class RandDogCommand : UnsplashRandomImageCommand
{
    protected override string Query => "dog";

    public RandDogCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
