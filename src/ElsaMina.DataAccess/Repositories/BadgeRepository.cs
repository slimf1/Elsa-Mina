using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class BadgeRepository : BaseRepository<Badge, Tuple<string, string>>, IBadgeRepository
{
    private readonly DbContext _dbContext;
    
    public BadgeRepository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<Badge> GetByIdAsync(Tuple<string, string> key,
        CancellationToken cancellationToken = default)
    {
        var (badgeId, roomId) = key;
        return await _dbContext.Set<Badge>()
            .Include(x => x.BadgeHolders)
            .ThenInclude(x => x.RoomSpecificUserData)
            .FirstOrDefaultAsync(x => x.Id == badgeId && x.RoomId == roomId, cancellationToken: cancellationToken);
    }

    public override async Task<IEnumerable<Badge>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Badge>()
            .Include(x => x.BadgeHolders)
            .ThenInclude(x => x.RoomSpecificUserData)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}