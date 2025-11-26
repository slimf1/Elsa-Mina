using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Clock;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Polls;

public class PollEndHandler : Handler
{
    private readonly IClockService _clockService;
    private readonly IBotDbContextFactory _dbContextFactory;

    public PollEndHandler(IClockService clockService, IBotDbContextFactory dbContextFactory)
    {
        _clockService = clockService;
        _dbContextFactory = dbContextFactory;
    }

    public override async Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        if (parts.Length < 3 || parts[1] != "html")
        {
            return;
        }

        var htmlContent = string.Join(" ", parts[2..]);
        if (htmlContent.Contains("Poll ended") || htmlContent.Contains("Sondage terminé"))
        {
            var poll = new SavedPoll
            {
                RoomId = roomId,
                Content = htmlContent,
                EndedAt = _clockService.CurrentUtcDateTime
            };
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            await dbContext.SavedPolls.AddAsync(poll, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}