using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Misc.Youtube;

public class YoutubeVideoPreviewViewModel : LocalizableViewModel
{
    public string VideoId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string ChannelTitle { get; init; }
    public DateTime PublishTime { get; init; }
    public string ThumbnailSource { get; init; }
    public int ThumbnailWidth { get; init; }
    public int ThumbnailHeight { get; init; }
}