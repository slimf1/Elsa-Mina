using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomSpecificUserDataRepository : BaseRepository<RoomSpecificUserData, Tuple<string, string>>, IRoomSpecificUserDataRepository
{
    private readonly DbContext _dbContext;

    public RoomSpecificUserDataRepository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<RoomSpecificUserData> GetByIdAsync(Tuple<string, string> key)
    {
        var (userId, roomId) = key;
        return await _dbContext.Set<RoomSpecificUserData>()
            .AsNoTracking()
            .Include(x => x.Badges)
            .ThenInclude(x => x.Badge)
            .FirstOrDefaultAsync(x => x.Id == userId && x.RoomId == roomId);
    }

    public override async Task<IEnumerable<RoomSpecificUserData>> GetAllAsync()
    {
        return await _dbContext.Set<RoomSpecificUserData>()
            .AsNoTracking()
            .Include(x => x.Badges)
            .ThenInclude(x => x.Badge)
            .ToListAsync();
    }
}