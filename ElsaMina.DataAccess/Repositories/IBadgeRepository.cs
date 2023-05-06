using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IBadgeRepository
{
    Task<Badge> GetByIdAsync(string id);
    Task<IEnumerable<Badge>> GetAllAsync();
    Task AddAsync(Badge badge);
    Task UpdateAsync(Badge badge);
    Task DeleteAsync(string id);
}