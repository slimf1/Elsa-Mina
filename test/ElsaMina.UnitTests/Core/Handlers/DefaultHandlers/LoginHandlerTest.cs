using ElsaMina.Core;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.Login;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Handlers.DefaultHandlers;

public class LoginHandlerTest
{
    private ILoginService _loginService;
    private IClient _client;
    private LoginHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _loginService = Substitute.For<ILoginService>();
        _client = Substitute.For<IClient>();
        _handler = new LoginHandler(_loginService, _client);
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldIgnore_WhenMessageIsNotChallstr()
    {
        // Arrange
        string[] message = ["", "othermessage", "data"];

        // Act
        await _handler.HandleReceivedMessageAsync(message);

        // Assert
        await _loginService.DidNotReceive().Login(Arg.Any<string>(), Arg.Any<CancellationToken>());
        _client.DidNotReceive().Send(Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldSendTrn_WhenLoginSucceeds()
    {
        // Arrange
        string[] message = ["", "challstr", "4", "nonce"];
        _loginService.Login("4|nonce", Arg.Any<CancellationToken>()).Returns(new LoginResponseDto
        {
            Assertion = "assertion",
            CurrentUser = new CurrentUserDto
            {
                IsLoggedIn = true,
                UserId = "lebot",
                Username = "LeBot"
            }
        });

        // Act
        await _handler.HandleReceivedMessageAsync(message);

        // Assert
        _client.Received(1).Send("|/trn LeBot,0,assertion");
    }
}
