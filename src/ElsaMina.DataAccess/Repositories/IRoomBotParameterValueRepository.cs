using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IRoomBotParameterValueRepository : IRepository<RoomBotParameterValue, Tuple<string, string>>
{
}