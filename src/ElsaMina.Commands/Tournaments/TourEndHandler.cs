using ElsaMina.Core.Handlers;

namespace ElsaMina.Commands.Tournaments;

public class TourEndHandler : Handler
{
    public override Task HandleReceivedMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}