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

    public abstract Task<T> GetByIdAsync(TKey key, CancellationToken cancellationToken = default);
    public abstract Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    protected DbSet<T> DbSet => _dbContext.Set<T>();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(key, cancellationToken);
        _dbContext.Set<T>().Remove(entity);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
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