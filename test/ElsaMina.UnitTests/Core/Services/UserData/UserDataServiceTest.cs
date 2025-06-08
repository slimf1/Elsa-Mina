using System.Net;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.UserData;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Core.Services.UserData;

public class UserDataServiceTest
{
    private IHttpService _httpService;
    private UserDataService _userDataService;

    [SetUp]
    public void Setup()
    {
        _httpService = Substitute.For<IHttpService>();
        _userDataService = new UserDataService(_httpService);
    }

    [Test]
    public async Task Test_GetUserData_ShouldReturnNull_WhenExceptionOccurs()
    {
        // Arrange
        const string userName = "testUser";
        _httpService.GetAsync<UserDataDto>(Arg.Any<string>()).Throws<Exception>();

        // Act
        var result = await _userDataService.GetUserData(userName);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Test_GetRegisterDate_ShouldReturnMinValue_WhenUserDataIsNull()
    {
        // Arrange
        const string userName = "testUser";
        _httpService.GetAsync<UserDataDto>(Arg.Any<string>()).ReturnsNull();

        // Act
        var result = await _userDataService.GetRegisterDateAsync(userName);

        // Assert
        Assert.That(result, Is.EqualTo(DateTimeOffset.MinValue));
    }

    [Test]
    public async Task Test_GetRegisterDate_ShouldReturnCorrectRegisterDate()
    {
        // Arrange
        const string userName = "testUser";
        var userData = new UserDataDto { RegisterTime = 1625760000 };
        _httpService.GetAsync<UserDataDto>(Arg.Any<string>()).Returns(new HttpResponse<UserDataDto>
        {
            Data = userData,
            StatusCode = HttpStatusCode.OK
        });

        // Act
        var result = await _userDataService.GetRegisterDateAsync(userName);

        // Assert
        Assert.That(result, Is.EqualTo(DateTimeOffset.Parse("07/08/2021 16:00:00Z")));
    }
}