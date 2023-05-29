using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class BadgeRepository : IRepository<Badge, Tuple<string, string>>
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    public BadgeRepository() : this(new BotDbContext())
    {
        
    }
    
    public BadgeRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Badge> GetByIdAsync(Tuple<string, string> key)
    {
        var (badgeId, roomId) = key;
        return await _dbContext.Set<Badge>()
            .Include(x => x.BadgeHolders)
            .FirstOrDefaultAsync(x => x.Id == badgeId && x.RoomId == roomId);
    }

    public async Task<IEnumerable<Badge>> GetAllAsync()
    {
        return await _dbContext.Set<Badge>()
            .Include(x => x.BadgeHolders)
            .ToListAsync();
    }

    public async Task AddAsync(Badge badge)
    {
        await _dbContext.Set<Badge>().AddAsync(badge);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Badge badge)
    {
        _dbContext.Set<Badge>().Update(badge);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tuple<string, string> key)
    {
        var badge = await GetByIdAsync(key);
        _dbContext.Set<Badge>().Remove(badge);
        await _dbContext.SaveChangesAsync();
    }

    private void Dispose(bool disposing)
    {
        if (!disposing || _disposed)
        {
            return;
        }
        _dbContext?.Dispose();
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BadgeRepository()
    {
        Dispose(false);
    }
}