using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randmouse")]
public class RandMouseCommand : UnsplashRandomImageCommand
{
    protected override string Query => "mouse";
    protected override string WarningKey => "rand_warning_mouse";

    public RandMouseCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
