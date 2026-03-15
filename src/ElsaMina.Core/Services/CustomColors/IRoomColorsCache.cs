namespace ElsaMina.Core.Services.CustomColors;

public interface IRoomColorsCache
{
    string GetColor(string userId);
    Task LoadAsync(CancellationToken cancellationToken = default);
}
