using System.Net;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Login;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.Test.Core.Services.Login;

public class LoginServiceTest
{
    private IHttpService _httpService;
    private IConfigurationManager _configurationManager;
    private LoginService _loginService;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _loginService = new LoginService(_httpService, _configurationManager);
    }

    [Test]
    public async Task Test_Login_ShouldReturnLoginResponse_WhenLoginSuccessful()
    {
        // Arrange
        var challstr = "sampleChallstr";
        var expectedResponse = new LoginResponseDto { /* set expected properties */ };
        var loginResponse = new HttpResponse<LoginResponseDto> { Data = expectedResponse };
        var config = new Configuration { Name = "user", Password = "password" };
        _configurationManager.Configuration.Returns(config);

        _httpService
            .PostUrlEncodedForm<LoginResponseDto>(Arg.Is(LoginService.LOGIN_URL), Arg.Any<Dictionary<string, string>>(), true)
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
        var config = new Configuration { Name = "user", Password = "password" };
        _configurationManager.Configuration.Returns(config);

        _httpService
            .PostUrlEncodedForm<LoginResponseDto>(Arg.Is(LoginService.LOGIN_URL), Arg.Any<Dictionary<string, string>>(), true)
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
        var config = new Configuration { Name = "user", Password = "password" };
        _configurationManager.Configuration.Returns(config);

        _httpService
            .PostUrlEncodedForm<LoginResponseDto>(Arg.Is(LoginService.LOGIN_URL), Arg.Any<Dictionary<string, string>>(), true)
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
        var config = new Configuration { Name = "user", Password = "password" };
        _configurationManager.Configuration.Returns(config);
        var expectedForm = new Dictionary<string, string>
        {
            ["challstr"] = challstr,
            ["name"] = config.Name,
            ["pass"] = config.Password,
            ["act"] = "login"
        };

        // Act
        await _loginService.Login(challstr);

        // Assert
        await _httpService.Received().PostUrlEncodedForm<LoginResponseDto>(
            Arg.Is(LoginService.LOGIN_URL),
            Arg.Is<Dictionary<string, string>>(form => 
                form["challstr"] == expectedForm["challstr"] &&
                form["name"] == expectedForm["name"] &&
                form["pass"] == expectedForm["pass"] &&
                form["act"] == expectedForm["act"]),
            true);
    }
}