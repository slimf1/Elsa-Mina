using System.Net;
using System.Text;
using System.Web;
using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Misc.Dailymotion;

[NamedCommand("dailymotion", "daily")]
public class DailymotionCommand : Command
{
    private const string BASE_API_URI = "https://api.dailymotion.com/videos";

    private readonly IHttpService _httpService;
    private readonly ITemplatesManager _templatesManager;

    public DailymotionCommand(IHttpService httpService, ITemplatesManager templatesManager)
    {
        _httpService = httpService;
        _templatesManager = templatesManager;
    }

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(context.Target))
        {
            context.ReplyLocalizedMessage("dailymotion_no_video_found");
            return;
        }
        
        var keywords = string.Join('+', context.Target.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        var queryParams = new Dictionary<string, string>
        {
            ["search"] = keywords,
            ["fields"] = "id,thumbnail_url,title,views_total,likes_total,explicit",
            ["limit"] = "5"
        };

        try
        {
            var result =
                await _httpService.GetAsync<VideoListResponse>(BASE_API_URI, queryParams,
                    cancellationToken: cancellationToken);
            if (result.StatusCode != HttpStatusCode.OK || result.Data == null || result.Data.List.Count == 0)
            {
                context.ReplyLocalizedMessage("dailymotion_no_video_found");
                return;
            }

            var videos = result.Data.List.OrderByDescending(video => video.ViewsTotal).ToList();
            var video = videos.FirstOrDefault(video => !video.Explicit);
            if (video == null)
            {
                context.ReplyLocalizedMessage("dailymotion_no_video_found");
                return;
            }

            var videoUrl = $"https://www.dailymotion.com/video/{HttpUtility.UrlEncode(video.Id)}";
            var template = await _templatesManager.GetTemplateAsync("Misc/Dailymotion/DailymotionVideoPreview",
                new DailymotionVideoPreviewViewModel
                {
                    Culture = context.Culture,
                    Title = video.Title,
                    VideoUrl = videoUrl,
                    ThumbnailUrl = video.ThumbnailUrl,
                    ViewsTotal = video.ViewsTotal,
                    LikesTotal = video.LikesTotal
                });
            context.SendHtmlIn(template.RemoveNewlines(), rankAware: true);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred fetching Dailymotion video");
            context.ReplyLocalizedMessage("dailymotion_fetch_error", exception.Message);
        }
    }
}