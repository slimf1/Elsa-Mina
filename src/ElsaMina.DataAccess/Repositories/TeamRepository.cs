using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class TeamRepository : BaseRepository<Team, string>, ITeamRepository
{
    private readonly DbContext _dbContext;

    public TeamRepository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<Team> GetByIdAsync(string key)
    {
        return await _dbContext.Set<Team>()
            .AsNoTracking()
            .Include(x => x.Rooms)
            .ThenInclude(x => x.RoomParameters)
            .FirstOrDefaultAsync(x => x.Id == key);
    }

    public override async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _dbContext.Set<Team>()
            .AsNoTracking()
            .Include(x => x.Rooms)
            .ThenInclude(x => x.RoomParameters)
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetTeamsFromRoom(string roomId)
    {
        return await _dbContext.Set<Team>()
            .AsNoTracking()
            .Include(x => x.Rooms)
            .ThenInclude(x => x.RoomParameters)
            .Where(x => x.Rooms.Any(room => room.RoomId == roomId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetTeamsFromRoomWithFormat(string roomId, string format)
    {
        return await _dbContext.Set<Team>()
            .AsNoTracking()
            .Include(x => x.Rooms)
            .ThenInclude(x => x.RoomParameters)
            .Where(x => x.Rooms.Any(room => room.RoomId == roomId))
            .Where(x => x.Format == format)
            .ToListAsync();
    }
}