using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface ISavedPollRepository : IRepository<SavedPoll, int>
{
    Task<IEnumerable<SavedPoll>> GetPollsByRoomIdAsync(string roomId, CancellationToken cancellationToken = default);
} 