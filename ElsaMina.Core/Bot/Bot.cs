using ElsaMina.Core.Client;

namespace ElsaMina.Core.Bot;

public class Bot : IBot
{
    private readonly IClient _client;

    public Bot(IClient client)
    {
        _client = client;
    }

    public void Start()
    {
        _client.Connect();
    }
}