using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface ITeamRepository : IRepository<Team, string>
{
    Task<IEnumerable<Team>> GetTeamsFromRoom(string roomId);
    Task<IEnumerable<Team>> GetTeamsFromRoomWithFormat(string roomId, string format);
}