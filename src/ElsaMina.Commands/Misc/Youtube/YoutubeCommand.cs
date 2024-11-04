using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using Serilog;

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
        var apiKey = _configurationManager.Configuration.YoutubeApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("Youtube API key is empty.");
            return;
        }
        var queryParams = new Dictionary<string, string>
        {
            ["part"] = "snippet",
            ["q"] = keywords,
            ["type"] = "video",
            ["key"] = apiKey
        };
        try
        {
            var response = await _httpService.Get<YouTubeSearchResponse>(YOUTUBE_API_URL, queryParams);
            
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve youtube search response.");
        }
        context.SendHtml("", rankAware: true);
    }
}