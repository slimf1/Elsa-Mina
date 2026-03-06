using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randshark")]
public class RandSharkCommand : UnsplashRandomImageCommand
{
    protected override string Query => "shark";
    protected override string WarningKey => "rand_warning_shark";

    public RandSharkCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
