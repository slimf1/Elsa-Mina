using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.TourConfigurator;

public class TourConfigService : ITourConfigService
{
    private readonly IBotDbContextFactory _dbContextFactory;

    public TourConfigService(IBotDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyList<TourConfig>> GetTourConfigsForRoomAsync(string roomId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.TourConfigs
            .Where(config => config.RoomId == roomId)
            .OrderBy(config => config.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<TourConfig> GetTourConfigAsync(string id, string roomId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.TourConfigs.FindAsync([id, roomId], cancellationToken);
    }

    public async Task SaveTourConfigAsync(TourConfig tourConfig,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await dbContext.TourConfigs.FindAsync([tourConfig.Id, tourConfig.RoomId], cancellationToken);
        if (existing == null)
        {
            await dbContext.TourConfigs.AddAsync(tourConfig, cancellationToken);
        }
        else
        {
            existing.Tier = tourConfig.Tier;
            existing.Format = tourConfig.Format;
            existing.Autostart = tourConfig.Autostart;
            existing.AutoDq = tourConfig.AutoDq;
            existing.TourName = tourConfig.TourName;
            existing.Teams = tourConfig.Teams;
            existing.Rules = tourConfig.Rules;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTourConfigAsync(string id, string roomId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await dbContext.TourConfigs.FindAsync([id, roomId], cancellationToken);
        if (existing != null)
        {
            dbContext.TourConfigs.Remove(existing);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
