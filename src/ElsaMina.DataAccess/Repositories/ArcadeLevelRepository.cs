using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class ArcadeLevelRepository : IArcadeLevelRepository
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    public ArcadeLevelRepository() : this(new BotDbContext())
    {
    }

    public ArcadeLevelRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ArcadeLevel> GetByIdAsync(string key)
    {
        return _dbContext.Set<ArcadeLevel>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == key);
    }

    public async Task<IEnumerable<ArcadeLevel>> GetAllAsync()
    {
        return await _dbContext.Set<ArcadeLevel>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(ArcadeLevel entity)
    {
        await _dbContext.Set<ArcadeLevel>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(ArcadeLevel entity)
    {
        _dbContext.Set<ArcadeLevel>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string key)
    {
        var entity = await GetByIdAsync(key);
        _dbContext.Set<ArcadeLevel>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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
}