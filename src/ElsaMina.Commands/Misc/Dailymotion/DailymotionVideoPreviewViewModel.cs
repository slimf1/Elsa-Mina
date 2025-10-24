using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Misc.Dailymotion;

public class DailymotionVideoPreviewViewModel : LocalizableViewModel
{
    public string VideoUrl { get; set; }
    public string Title { get; set; }
    public string ThumbnailUrl { get; set; }
    public int ViewsTotal { get; set; }
    public int LikesTotal { get; set; }
}