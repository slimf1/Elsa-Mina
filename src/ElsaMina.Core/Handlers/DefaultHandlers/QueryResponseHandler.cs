using ElsaMina.Core.Services.UserDetails;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public sealed class QueryResponseHandler : Handler
{
    private readonly IUserDetailsManager _userDetailsManager;

    public QueryResponseHandler(IUserDetailsManager userDetailsManager)
    {
        _userDetailsManager = userDetailsManager;
    }

    public override string Identifier => nameof(QueryResponseHandler);
    
    protected override Task Execute(string[] parts, string roomId = null)
    {
        if (parts.Length >= 2 && parts[1] == "queryresponse")
        {
            if (parts[2] == "userdetails")
            {
                _userDetailsManager.HandleReceivedUserDetails(parts[3]);
            }
        }

        return Task.CompletedTask;
    }
}