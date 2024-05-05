using ElsaMina.Core.Services.UserDetails;

namespace ElsaMina.Core.Parsers.DefaultParsers;

public sealed class QueryResponseParser : Parser
{
    private readonly IUserDetailsManager _userDetailsManager;

    public QueryResponseParser(IUserDetailsManager userDetailsManager)
    {
        _userDetailsManager = userDetailsManager;
    }

    public override string Identifier => nameof(QueryResponseParser);
    
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