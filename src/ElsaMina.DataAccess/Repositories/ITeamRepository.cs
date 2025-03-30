using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface ITeamRepository : IRepository<Team, string>
{
    Task<IEnumerable<Team>> GetTeamsFromRoom(string roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Team>> GetTeamsFromRoomWithFormat(string roomId, string format, CancellationToken cancellationToken = default);
}