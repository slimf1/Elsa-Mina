using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class BadgeHoldingRepository : BaseRepository<BadgeHolding, Tuple<string, string, string>>,
    IBadgeHoldingRepository
{
    private readonly DbContext _dbContext;

    public BadgeHoldingRepository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<BadgeHolding> GetByIdAsync(Tuple<string, string, string> key)
    {
        var (badgeId, userId, roomId) = key;
        return await _dbContext.Set<BadgeHolding>()
            .AsNoTracking()
            .Include(x => x.Badge)
            .Include(x => x.RoomSpecificUserData)
            .FirstOrDefaultAsync(x => x.BadgeId == badgeId
                                      && x.RoomId == roomId
                                      && x.UserId == userId);
    }

    public override async Task<IEnumerable<BadgeHolding>> GetAllAsync()
    {
        return await _dbContext.Set<BadgeHolding>()
            .AsNoTracking()
            .Include(x => x.Badge)
            .Include(x => x.RoomSpecificUserData)
            .ToListAsync();
    }
}