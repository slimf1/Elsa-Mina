using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Misc.RandomImages;

public abstract class UnsplashRandomImageCommand : Command
{
    protected abstract string Query { get; }
    protected virtual string WarningKey => null;

    private readonly IUnsplashService _unsplashService;

    protected UnsplashRandomImageCommand(IUnsplashService unsplashService)
    {
        _unsplashService = unsplashService;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var photoUrl = await _unsplashService.GetRandomPhotoUrlAsync(Query, cancellationToken);
        if (string.IsNullOrEmpty(photoUrl))
        {
            Log.Error("Unsplash returned no photo for query: {Query}", Query);
            context.ReplyLocalizedMessage("random_image_error");
            return;
        }

        var img = $"<img src=\"{photoUrl}\" style=\"transform:rotate(0deg);\" width=\"300\" height=\"200\">";
        var html = WarningKey != null
            ? $"<details><summary>{context.GetString(WarningKey)}</summary>{img}</details>"
            : img;

        context.ReplyHtml(html, rankAware: true);
    }
}
