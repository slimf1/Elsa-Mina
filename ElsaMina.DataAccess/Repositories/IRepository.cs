namespace ElsaMina.DataAccess.Repositories;

public interface IRepository<T, in TKey> : IDisposable
{
    Task<T> GetByIdAsync(TKey key);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(TKey key);
}