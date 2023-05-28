using ElsaMina.DataAccess.Models;
using Serilog;

namespace ElsaMina.DataAccess.Repositories;

public class CachedRepository<TRepository, T, TKey> : IRepository<T, TKey>
    where TRepository : IRepository<T, TKey>
    where T : IKeyed<TKey>
{
    private readonly TRepository _repository;
    private readonly ILogger _logger;

    private readonly Dictionary<TKey, T> _cache = new();
    private bool _fetchedAll;
    private bool _disposed;

    public CachedRepository(ILogger logger, TRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<T> GetByIdAsync(TKey key)
    {
        if (_cache.TryGetValue(key, out var cachedEntity))
        {
            return cachedEntity;
        }

        try
        {
            var entity = await _repository.GetByIdAsync(key);
            if (entity != null)
            {
                _cache[key] = entity;
            }

            return entity;
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occured while fetching an entity");
            return default;
        }
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
        _cache[entity.Key] = entity;
        await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        _cache[entity.Key] = entity;
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(TKey id)
    {
        _cache.Remove(id);
        await _repository.DeleteAsync(id);
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