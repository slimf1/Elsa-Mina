using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class SavedPollRepository : BaseRepository<SavedPoll, int>, ISavedPollRepository
{
    public SavedPollRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<SavedPoll> GetByIdAsync(int key, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(poll => poll.RoomInfo)
            .FirstOrDefaultAsync(poll => poll.Id == key, cancellationToken);
    }

    public override async Task<IEnumerable<SavedPoll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(poll => poll.RoomInfo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SavedPoll>> GetPollsByRoomIdAsync(string roomId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(poll => poll.RoomInfo)
            .Where(poll => poll.RoomId == roomId)
            .ToListAsync(cancellationToken);
    }
}