using ElsaMina.Core.Client;

namespace ElsaMina.Core.Bot;

public class Bot : IBot
{
    private readonly IClient _client;

    public Bot(IClient client)
    {
        _client = client;
    }

    public async Task Start()
    {
        _client.MessageReceived?.Subscribe(HandleReceivedMessage);
        await _client.Connect();
    }

    private void HandleReceivedMessage(string message)
    {
        Console.WriteLine(message);
    }
}