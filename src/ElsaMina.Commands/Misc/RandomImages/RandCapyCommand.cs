using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Misc.RandomImages;

[NamedCommand("randcapy")]
public class RandCapyCommand : UnsplashRandomImageCommand
{
    protected override string Query => "capybara";

    public RandCapyCommand(IUnsplashService unsplashService) : base(unsplashService) { }
}
