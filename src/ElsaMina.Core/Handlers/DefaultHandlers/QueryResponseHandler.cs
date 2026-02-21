using ElsaMina.Core.Services.BattleTracker;
using ElsaMina.Core.Services.UserDetails;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public sealed class QueryResponseHandler : Handler
{
    private readonly IActiveBattlesManager _activeBattlesManager;
    private readonly IUserDetailsManager _userDetailsManager;

    public QueryResponseHandler(IUserDetailsManager userDetailsManager, IActiveBattlesManager activeBattlesManager)
    {
        _userDetailsManager = userDetailsManager;
        _activeBattlesManager = activeBattlesManager;
    }

    public override Task HandleReceivedMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default)
    {
        if (parts.Length < 4 || parts[1] != "queryresponse")
        {
            return Task.CompletedTask;
        }

        if (parts[2] == "userdetails")
        {
            _userDetailsManager.HandleReceivedUserDetails(parts[3]);
        }
        else if (parts[2] == "roomlist")
        {
            _activeBattlesManager.HandleReceivedRoomList(parts[3]);
        }

        return Task.CompletedTask;
    }
}
