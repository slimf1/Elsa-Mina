using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public abstract class BaseRepository<T, TKey> : IRepository<T, TKey> where T : class, IKeyed<TKey>
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    protected BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public abstract Task<T> GetByIdAsync(TKey key);
    public abstract Task<IEnumerable<T>> GetAllAsync();
    
    public async Task AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(entity).State = EntityState.Detached;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(entity).State = EntityState.Detached;
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(TKey key)
    {
        var entity = await GetByIdAsync(key);
        _dbContext.Set<T>().Remove(entity);
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

    ~BaseRepository()
    {
        Dispose(false);
    }
}