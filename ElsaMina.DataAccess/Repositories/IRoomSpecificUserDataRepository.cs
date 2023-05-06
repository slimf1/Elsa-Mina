using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IRoomSpecificUserDataRepository
{
    Task<RoomSpecificUserData> GetByIdAsync(string id);
    Task<IEnumerable<RoomSpecificUserData>> GetAllAsync();
    Task AddAsync(RoomSpecificUserData roomSpecificUserData);
    Task UpdateAsync(RoomSpecificUserData roomSpecificUserData);
    Task DeleteAsync(string id);
}