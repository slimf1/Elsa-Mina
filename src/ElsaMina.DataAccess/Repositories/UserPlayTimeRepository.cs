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

    public override async Task<UserPlayTime> GetByIdAsync(Tuple<string, string> key)
    {
        var (userId, roomId) = key;
        return await _dbContext.Set<UserPlayTime>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RoomId == roomId);
    }

    public override async Task<IEnumerable<UserPlayTime>> GetAllAsync()
    {
        return await _dbContext.Set<UserPlayTime>()
            .AsNoTracking()
            .ToListAsync();
    }
}