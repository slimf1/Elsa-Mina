using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IAddedCommandRepository : IDisposable
{
    Task<AddedCommand> GetByIdAsync(string id);
    Task<IEnumerable<AddedCommand>> GetAllAsync();
    Task AddAsync(AddedCommand addedCommand);
    Task UpdateAsync(AddedCommand addedCommand);
    Task DeleteAsync(string id);
}