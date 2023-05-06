using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class BadgeRepository : IBadgeRepository
{
    private readonly DbContext _dbContext;

    public BadgeRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Badge> GetByIdAsync(string id)
    {
        return await _dbContext.Set<Badge>()
            .Include(x => x.BadgeHolders)
            .FirstOrDefaultAsync(x => x.Id == id);
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

    public async Task DeleteAsync(string id)
    {
        var badge = await GetByIdAsync(id);
        _dbContext.Set<Badge>().Remove(badge);
        await _dbContext.SaveChangesAsync();
    }
}