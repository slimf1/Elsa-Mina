namespace ElsaMina.Core.Services.Start;

public interface IStartManager
{
    Task LoadStaticDataAsync(CancellationToken cancellationToken = default);
}