using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IRoomSpecificUserDataRepository : IRepository<RoomSpecificUserData, Tuple<string, string>>
{
    
}