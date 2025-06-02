using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Clock;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Polls;

public class PollEndHandler : Handler
{
    private readonly ISavedPollRepository _savedPollRepository;
    private readonly IClockService _clockService;

    public PollEndHandler(ISavedPollRepository savedPollRepository, IClockService clockService)
    {
        _savedPollRepository = savedPollRepository;
        _clockService = clockService;
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
            await _savedPollRepository.AddAsync(poll, cancellationToken);
        }
    }
}