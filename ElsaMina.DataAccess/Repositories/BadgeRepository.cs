namespace ElsaMina.DataAccess.Repositories;

public class BadgeRepository : IBadgeRepository
{
    private readonly BotDbContext _context;
    private bool _disposed;
    
    public BadgeRepository(BotDbContext context)
    {
        _context = context;
    }
}