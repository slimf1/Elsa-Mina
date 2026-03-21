using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.TourConfigurator;

public interface ITourConfigService
{
    Task<IReadOnlyList<TourConfig>> GetTourConfigsForRoomAsync(string roomId, CancellationToken cancellationToken = default);
    Task<TourConfig> GetTourConfigAsync(string id, string roomId, CancellationToken cancellationToken = default);
    Task SaveTourConfigAsync(TourConfig tourConfig, CancellationToken cancellationToken = default);
    Task DeleteTourConfigAsync(string id, string roomId, CancellationToken cancellationToken = default);
}
