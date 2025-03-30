using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class UserPlayTimeRepository : BaseRepository<UserPlayTime, Tuple<string, string>>, IUserPlayTimeRepository
{
    private readonly DbContext _dbContext;

    public UserPlayTimeRepository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<UserPlayTime> GetByIdAsync(Tuple<string, string> key,
        CancellationToken cancellationToken = default)
    {
        var (userId, roomId) = key;
        return await _dbContext.Set<UserPlayTime>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RoomId == roomId, cancellationToken: cancellationToken);
    }

    public override async Task<IEnumerable<UserPlayTime>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<UserPlayTime>()
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}