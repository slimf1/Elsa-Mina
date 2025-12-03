using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Misc.Genius;

public class GeniusSongPanelViewModel : LocalizableViewModel
{
    public string Title { get; set; }
    public string ArtistName { get; set; }
    public string ThumbnailUrl { get; set; }
    public int ThumbnailHeight { get; set; }
    public int ThumbnailWidth { get; set; }
    public string LyricsUrl { get; set; }
    public int PageViews { get; set; }
    public string ReleaseDate { get; set; }
}