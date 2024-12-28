using System.Globalization;
using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Misc.Youtube;

[NamedCommand("youtube", Aliases = ["yt", "ytb"])]
public class YoutubeCommand : Command
{
    private const int THUMBNAIL_WIDTH = 160;
    private const int THUMBNAIL_HEIGHT = 90;
    private const string YOUTUBE_API_URL = "https://www.googleapis.com/youtube/v3/search";
    public const int DESCRIPTION_MAX_LENGTH = 100;

    private readonly IHttpService _httpService;
    private readonly IConfigurationManager _configurationManager;
    private readonly ITemplatesManager _templatesManager;

    public YoutubeCommand(IHttpService httpService,
        IConfigurationManager configurationManager,
        ITemplatesManager templatesManager)
    {
        _httpService = httpService;
        _configurationManager = configurationManager;
        _templatesManager = templatesManager;
    }

    public override Rank RequiredRank => Rank.Regular;

    public override async Task Run(IContext context)
    {
        var keywords = string.Join('+', context.Target.Split(' '));
        var apiKey = _configurationManager.Configuration.YoutubeApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Logger.Error("Youtube API key is empty.");
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
            var results = response.Data;
            if (results?.Items == null || results.Items.Count == 0)
            {
                Logger.Error("Youtube API returned no results.");
                context.ReplyLocalizedMessage("youtube_no_results");
                return;
            }

            var firstVideo = results.Items[0];
            var firstVideoSnippet = firstVideo.Snippet;
            var template = await _templatesManager.GetTemplate("Misc/Youtube/YoutubeVideoPreview",
                new YoutubeVideoPreviewViewModel
                {
                    Culture = context.Culture,
                    ChannelTitle = firstVideoSnippet.ChannelTitle,
                    Description = firstVideoSnippet.Description,
                    PublishTime = DateTime.ParseExact(
                        firstVideoSnippet.PublishTime,
                        "yyyy-MM-ddTHH:mm:ssZ",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind
                    ),
                    Title = firstVideoSnippet.Title,
                    VideoId = firstVideo.Id.VideoIdValue,
                    ThumbnailSource = firstVideoSnippet.Thumbnails.Medium.Url,
                    ThumbnailWidth = THUMBNAIL_WIDTH,
                    ThumbnailHeight = THUMBNAIL_HEIGHT
                });

            context.SendHtml(template.RemoveNewlines(), rankAware: true);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to retrieve youtube search response.");
            context.ReplyLocalizedMessage("youtube_error_occurred");
        }
    }
}