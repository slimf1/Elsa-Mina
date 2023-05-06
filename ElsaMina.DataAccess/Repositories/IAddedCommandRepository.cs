using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IAddedCommandRepository : IDisposable
{
    Task<AddedCommand> GetByIdAsync(string commandId, string roomId);
    Task<IEnumerable<AddedCommand>> GetAllAsync(string roomId);
    Task AddAsync(AddedCommand addedCommand);
    Task UpdateAsync(AddedCommand addedCommand);
    Task DeleteAsync(string commandId, string roomId);
}