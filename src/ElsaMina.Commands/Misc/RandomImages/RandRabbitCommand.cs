using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randrabbit")]
public class RandRabbitCommand : UnsplashRandomImageCommand
{
    protected override string Query => "rabbit";

    public RandRabbitCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
