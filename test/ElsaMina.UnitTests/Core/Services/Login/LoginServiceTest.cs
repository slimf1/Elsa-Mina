using System.Net;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Login;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Core.Services.Login;

public class LoginServiceTest
{
    private IHttpService _httpService;
    private IConfiguration _configuration;
    private LoginService _loginService;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _configuration = Substitute.For<IConfiguration>();
        _loginService = new LoginService(_httpService, _configuration);
    }

    [Test]
    public async Task Test_Login_ShouldReturnLoginResponse_WhenLoginSuccessful()
    {
        // Arrange
        var challstr = "sampleChallstr";
        var expectedResponse = new LoginResponseDto { /* set expected properties */ };
        var loginResponse = new HttpResponse<LoginResponseDto> { Data = expectedResponse };
        _configuration.Name.Returns("user");
        _configuration.Password.Returns("password");

        _httpService
            .PostUrlEncodedFormAsync<LoginResponseDto>(Arg.Is(LoginService.LOGIN_URL), Arg.Any<Dictionary<string, string>>(), true)
            .Returns(loginResponse);

        // Act
        var result = await _loginService.Login(challstr);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task Test_Login_ShouldReturnNull_WhenHttpExceptionThrown()
    {
        // Arrange
        var challstr = "sampleChallstr";
        _configuration.Name.Returns("user");
        _configuration.Password.Returns("password");

        _httpService
            .PostUrlEncodedFormAsync<LoginResponseDto>(Arg.Is(LoginService.LOGIN_URL), Arg.Any<Dictionary<string, string>>(), true)
            .Throws(new HttpException(HttpStatusCode.InternalServerError, "Internal Server Error"));

        // Act
        var result = await _loginService.Login(challstr);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Test_Login_ShouldReturnNull_WhenGeneralExceptionThrown()
    {
        // Arrange
        var challstr = "sampleChallstr";
        _configuration.Name.Returns("user");
        _configuration.Password.Returns("password");

        _httpService
            .PostUrlEncodedFormAsync<LoginResponseDto>(Arg.Is(LoginService.LOGIN_URL), Arg.Any<Dictionary<string, string>>(), true)
            .Throws(new Exception("Unexpected error"));

        // Act
        var result = await _loginService.Login(challstr);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Test_Login_ShouldSendCorrectFormData()
    {
        // Arrange
        var challstr = "sampleChallstr";
        _configuration.Name.Returns("user");
        _configuration.Password.Returns("password");
        var expectedForm = new Dictionary<string, string>
        {
            ["challstr"] = challstr,
            ["name"] = "user",
            ["pass"] = "password",
            ["act"] = "login"
        };

        // Act
        await _loginService.Login(challstr);

        // Assert
        await _httpService.Received().PostUrlEncodedFormAsync<LoginResponseDto>(
            Arg.Is(LoginService.LOGIN_URL),
            Arg.Is<Dictionary<string, string>>(form => 
                form["challstr"] == expectedForm["challstr"] &&
                form["name"] == expectedForm["name"] &&
                form["pass"] == expectedForm["pass"] &&
                form["act"] == expectedForm["act"]),
            true);
    }
}