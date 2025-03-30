using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class AddedCommandRepository : BaseRepository<AddedCommand, Tuple<string, string>>, IAddedCommandRepository
{
    private readonly DbContext _dbContext;
    
    public AddedCommandRepository(DbContext context) : base(context)
    {
        _dbContext = context;
    }
    
    public override async Task<AddedCommand> GetByIdAsync(Tuple<string, string> key,
        CancellationToken cancellationToken = default)
    {
        var (commandId, roomId) = key;
        return await _dbContext.Set<AddedCommand>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == commandId && x.RoomId == roomId, cancellationToken: cancellationToken);
    }

    public override async Task<IEnumerable<AddedCommand>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<AddedCommand>()
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}