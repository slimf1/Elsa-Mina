using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IRoomParametersRepository : IDisposable
{
    Task<RoomParameters> GetByIdAsync(string id);
    Task<IEnumerable<RoomParameters>> GetAllAsync();
    Task AddAsync(RoomParameters roomParameters);
    Task UpdateAsync(RoomParameters roomParameters);
    Task DeleteAsync(string id);
}