using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomInfoRepository : BaseRepository<RoomInfo, string>, IRoomInfoRepository
{
    private readonly DbContext _dbContext;
    
    public RoomInfoRepository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<RoomInfo> GetByIdAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<RoomInfo>()
            .AsNoTracking()
            .Include(x => x.Teams)
            .Include(x => x.ParameterValues)
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken: cancellationToken);
    }

    public override async Task<IEnumerable<RoomInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<RoomInfo>()
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}