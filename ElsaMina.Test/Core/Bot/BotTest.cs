using ElsaMina.Core.Client;
using NSubstitute;

namespace ElsaMina.Test.Core.Bot;

public class BotTest
{
    private ElsaMina.Core.Bot.Bot _bot;

    private IClient _client;
    
    [SetUp]
    public void SetUp()
    {
        _client = Substitute.For<IClient>();
        
        _bot = new ElsaMina.Core.Bot.Bot(_client);
    }

    [Test]
    public async Task Test_Start_ShouldConnect()
    {
        // Arrange
        var observable = Substitute.For<IObservable<string>>();
        _client.MessageReceived.Returns(observable);

        // Act
        await _bot.Start();
        
        // Assert
        await _client.Received(1).Connect();
        observable.Received(1).Subscribe(Arg.Any<IObserver<string>>());
    }
}