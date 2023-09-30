using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class BadgeHoldingRepository : IBadgeHoldingRepository
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    public BadgeHoldingRepository() : this(new BotDbContext())
    {
        
    }
    
    public BadgeHoldingRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BadgeHolding> GetByIdAsync(Tuple<string, string, string> key)
    {
        var (badgeId, userId, roomId) = key;
        return await _dbContext.Set<BadgeHolding>()
            .Include(x => x.Badge)
            .Include(x => x.RoomSpecificUserData)
            .FirstOrDefaultAsync(x => x.BadgeId == badgeId
                                      && x.RoomId == roomId
                                      && x.UserId == userId);
    }

    public async Task<IEnumerable<BadgeHolding>> GetAllAsync()
    {
        return await _dbContext.Set<BadgeHolding>()
            .Include(x => x.Badge)
            .Include(x => x.RoomSpecificUserData)
            .ToListAsync();
    }

    public async Task AddAsync(BadgeHolding entity)
    {
        await _dbContext.Set<BadgeHolding>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(BadgeHolding entity)
    {
        _dbContext.Set<BadgeHolding>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tuple<string, string, string> key)
    {
        var entity = await GetByIdAsync(key);
        _dbContext.Set<BadgeHolding>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    private void Dispose(bool disposing)
    {
        if (!disposing || _disposed)
        {
            return;
        }
        _dbContext.Dispose();
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BadgeHoldingRepository()
    {
        Dispose(false);
    }
}