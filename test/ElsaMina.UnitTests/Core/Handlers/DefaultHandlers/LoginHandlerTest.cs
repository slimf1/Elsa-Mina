using ElsaMina.Core;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Core.Handlers.DefaultHandlers;

public class LoginHandlerTest
{
    private ILoginService _loginService;
    private IConfiguration _configuration;
    private IClient _client;
    private ISystemService _systemService;
    private LoginHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _loginService = Substitute.For<ILoginService>();
        _configuration = Substitute.For<IConfiguration>();
        _client = Substitute.For<IClient>();
        _systemService = Substitute.For<ISystemService>();

        _handler = new LoginHandler(_loginService, _configuration, _systemService, _client);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldLogin_WhenChallstrHasBeenReceived()
    {
        // Arrange
        string[] message = ["", "challstr", "4", "nonce"];
        _configuration.Name.Returns("LeBot");
        _loginService.Login("4|nonce").Returns(new LoginResponseDto
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
        _systemService.DidNotReceive().Kill();
        _client.Received(1).Send("|/trn LeBot,0,assertion");
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldKill_WhenLoginFails()
    {
        // Arrange
        string[] message = ["", "challstr", "4", "nonce"];
        _configuration.Name.Returns("LeBot");
        _loginService.Login("4|nonce").ReturnsNull();

        // Act
        await _handler.HandleReceivedMessageAsync(message);

        // Assert
        _systemService.Received(1).Kill();
        _client.DidNotReceive().Send(Arg.Any<string>());
    }
}