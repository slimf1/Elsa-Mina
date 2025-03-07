using ElsaMina.Core.Services.CustomColors;
using ElsaMina.Core.Services.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.Test.Core.Services.CustomColors;

public class CustomColorsManagerTest
{
    private IHttpService _httpService;
    private CustomColorsManager _customColorsManager;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _customColorsManager = new CustomColorsManager(_httpService);
    }

    [Test]
    public async Task Test_FetchCustomColors_ShouldSetCustomColorsMapping_WhenResponseIsSuccessful()
    {
        // Arrange
        var expectedColors = new Dictionary<string, string>
        {
            { "user1", "#FF5733" },
            { "user2", "#33FF57" }
        };
        var response = new HttpResponse<Dictionary<string, string>> { Data = expectedColors };
        _httpService.GetAsync<Dictionary<string, string>>(Arg.Is(CustomColorsManager.CUSTOM_COLORS_FILE_URL)).Returns(response);

        // Act
        await _customColorsManager.FetchCustomColors();

        // Assert
        Assert.That(_customColorsManager.CustomColorsMapping, Is.Not.Null);
        Assert.That(_customColorsManager.CustomColorsMapping, Has.Count.EqualTo(2));
        Assert.That(_customColorsManager.CustomColorsMapping, Is.EquivalentTo(expectedColors));
    }

    [Test]
    public async Task Test_FetchCustomColors_ShouldLogInformation_WhenColorsAreFetched()
    {
        // Arrange
        var expectedColors = new Dictionary<string, string> { { "user1", "#FF5733" } };
        var response = new HttpResponse<Dictionary<string, string>> { Data = expectedColors };
        _httpService.GetAsync<Dictionary<string, string>>(Arg.Is(CustomColorsManager.CUSTOM_COLORS_FILE_URL)).Returns(response);

        // Act
        await _customColorsManager.FetchCustomColors();

        // Assert
        Assert.That(_customColorsManager.CustomColorsMapping, Has.Count.EqualTo(1));
        Assert.That(_customColorsManager.CustomColorsMapping["user1"], Is.EqualTo("#FF5733"));
    }

    [Test]
    public async Task Test_FetchCustomColors_ShouldLogError_WhenExceptionThrown()
    {
        // Arrange
        _httpService.GetAsync<Dictionary<string, string>>(Arg.Is(CustomColorsManager.CUSTOM_COLORS_FILE_URL)).Throws(new Exception("Network error"));

        // Act
        await _customColorsManager.FetchCustomColors();

        // Assert
        Assert.That(_customColorsManager.CustomColorsMapping, Is.Empty);
    }

    [Test]
    public void Test_FetchCustomColors_ShouldNotThrowException_WhenExceptionOccurs()
    {
        // Arrange
        _httpService.GetAsync<Dictionary<string, string>>(Arg.Is(CustomColorsManager.CUSTOM_COLORS_FILE_URL)).Throws(new Exception("Network error"));

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _customColorsManager.FetchCustomColors());
    }
}