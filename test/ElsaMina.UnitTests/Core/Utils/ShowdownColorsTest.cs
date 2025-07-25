using System.Drawing;
using ElsaMina.Core.Services.CustomColors;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Utils;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Utils;

public class ShowdownColorsTests
{
    private ICustomColorsManager _customColorsManager;

    [SetUp]
    public void SetUp()
    {
        _customColorsManager = Substitute.For<ICustomColorsManager>();
        var containerService = Substitute.For<IDependencyContainerService>();
        containerService.Resolve<ICustomColorsManager>().Returns(_customColorsManager);
        DependencyContainerService.Current = containerService;
    }

    [TearDown]
    public void TearDown()
    {
        DependencyContainerService.Current = null;
    }

    [Test]
    public void Test_ToColor_ShouldGenerateConsistentColor_ForSameString()
    {
        // Arrange
        const string input = "consistent";

        // Act
        var color1 = input.ToColor();
        var color2 = input.ToColor();

        // Assert
        Assert.That(color1, Is.EqualTo(color2)); // Value equality
    }

    [Test]
    public void Test_ToHexString_ShouldReturnCorrectFormat_WhenCalled()
    {
        // Arrange
        var color = Color.FromArgb(255, 128, 64);

        // Act
        var hexString = color.ToHexString();

        // Assert
        Assert.That(hexString, Is.EqualTo("#FF8040"));
    }

    [Test]
    public void Test_ToHslString_ShouldReturnCorrectHslString_WhenCalled()
    {
        // Arrange
        var color = Color.FromArgb(255, 128, 64);

        // Act
        var hslString = color.ToHslString();

        // Assert
        Assert.That(hslString, Does.StartWith("HSL("));
    }

    [Test]
    public void Test_ToRgbString_ShouldReturnCorrectRgbString_WhenCalled()
    {
        // Arrange
        var color = Color.FromArgb(255, 128, 64);

        // Act
        var rgbString = color.ToRgbString();

        // Assert
        Assert.That(rgbString, Is.EqualTo("RGB(255, 128, 64)"));
    }

    [Test]
    public void Test_ToColorHexCodeWithCustoms_ShouldUseCustomColor_WhenCustomColorExists()
    {
        // Arrange
        var userName = "customUser";
        var customColorUsername = "speks";
        _customColorsManager.CustomColorsMapping
            .Returns(new Dictionary<string, string> { { userName.ToLowerAlphaNum(), customColorUsername } });

        // Act
        var result = userName.ToColorHexCodeWithCustoms();

        // Assert
        Assert.That(result, Is.EqualTo(customColorUsername.ToColor().ToHexString()));
    }

    [Test]
    public void Test_ToColorHexCodeWithCustoms_ShouldFallbackToGeneratedColor_WhenNoCustomColorExists()
    {
        // Arrange
        var userName = "fallbackUser";
        _customColorsManager.CustomColorsMapping
            .Returns(new Dictionary<string, string>()); // No custom mapping

        // Act
        var result = userName.ToColorHexCodeWithCustoms();

        // Assert
        Assert.That(result, Does.StartWith("#"));
    }
}