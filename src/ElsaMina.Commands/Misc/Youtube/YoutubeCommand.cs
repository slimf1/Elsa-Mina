using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Misc.Youtube;

[NamedCommand("youtube", Aliases = ["yt", "ytb"])]
public class YoutubeCommand : Command
{
    private const string YOUTUBE_API_URL = "https://www.googleapis.com/youtube/v3/search";
    
    private readonly IHttpService _httpService;
    private readonly IConfigurationManager _configurationManager;

    public YoutubeCommand(IHttpService httpService,
        IConfigurationManager configurationManager)
    {
        _httpService = httpService;
        _configurationManager = configurationManager;
    }

    public override async Task Run(IContext context)
    {
        var keywords = string.Join('+', context.Target.Split(' '));
        var queryParams = new Dictionary<string, string>
        {
            ["part"] = "snippet",
            ["q"] = keywords,
            ["type"] = "video",
            ["key"] = _configurationManager.Configuration.YoutubeApiKey
        };
        try
        {
            var response = await _httpService.Get<YouTubeSearchResponse>(YOUTUBE_API_URL, queryParams);
            
        }
        catch (Exception ex)
        {
            
        }
        context.SendHtml("", rankAware: true);
    }
}