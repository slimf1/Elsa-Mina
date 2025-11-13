using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomInfoRepository : BaseRepository<RoomInfo, string>, IRoomInfoRepository
{
    public RoomInfoRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<RoomInfo> GetByIdAsync(string key, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Teams)
            .Include(x => x.ParameterValues)
            .Include(x => x.PollHistory)
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken: cancellationToken);
    }

    public override async Task<IEnumerable<RoomInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Teams)
            .Include(x => x.ParameterValues)
            .Include(x => x.PollHistory)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}