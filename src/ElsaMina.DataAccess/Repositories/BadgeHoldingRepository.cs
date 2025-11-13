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

    public override async Task<BadgeHolding> GetByIdAsync(Tuple<string, string, string> key,
        CancellationToken cancellationToken = default)
    {
        var (badgeId, userId, roomId) = key;
        return await _dbContext.Set<BadgeHolding>()
            .Include(x => x.Badge)
            .Include(x => x.RoomSpecificUserData)
            .FirstOrDefaultAsync(x => x.BadgeId == badgeId
                                      && x.RoomId == roomId
                                      && x.UserId == userId, cancellationToken: cancellationToken);
    }

    public override async Task<IEnumerable<BadgeHolding>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<BadgeHolding>()
            .Include(x => x.Badge)
            .Include(x => x.RoomSpecificUserData)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}