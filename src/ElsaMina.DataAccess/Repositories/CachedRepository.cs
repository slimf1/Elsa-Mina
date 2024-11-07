using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public class CachedRepository<TRepository, T, TKey> : IRepository<T, TKey>
    where TRepository : IRepository<T, TKey>
    where T : IKeyed<TKey>
{
    private readonly TRepository _repository;

    private readonly Dictionary<TKey, T> _cache = new();
    private bool _fetchedAll;
    private bool _disposed;

    public CachedRepository(TRepository repository)
    {
        _repository = repository;
    }

    public async Task<T> GetByIdAsync(TKey key)
    {
        if (_cache.TryGetValue(key, out var cachedEntity))
        {
            return cachedEntity;
        }

        var entity = await _repository.GetByIdAsync(key);
        if (entity != null)
        {
            _cache[key] = entity;
        }

        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var entities = (await _repository.GetAllAsync()).ToList();
        if (_fetchedAll)
        {
            return _cache.Values;
        }

        foreach (var entity in entities)
        {
            _cache[entity.Key] = entity;
        }

        _fetchedAll = true;
        return entities;
    }

    public async Task AddAsync(T entity)
    {
        await _repository.AddAsync(entity);
        _cache[entity.Key] = entity;
    }

    public async Task UpdateAsync(T entity)
    {
        await _repository.UpdateAsync(entity);
        _cache[entity.Key] = entity;
    }

    public async Task DeleteAsync(TKey key)
    {
        await _repository.DeleteAsync(key);
        _cache.Remove(key);
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

        _repository.Dispose();
        _disposed = true;
    }

    ~CachedRepository()
    {
        Dispose(false);
    }
}