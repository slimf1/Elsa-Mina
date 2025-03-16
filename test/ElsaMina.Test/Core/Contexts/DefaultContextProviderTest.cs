using System.Globalization;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.Test.Core.Contexts;

public class DefaultContextProviderTest
{
    private DefaultContextProvider _contextProvider;
    private IConfigurationManager _configurationManager;
    private IResourcesService _resourcesService;
    private IRoomsManager _roomsManager;

    [SetUp]
    public void SetUp()
    {
        _configurationManager = Substitute.For<IConfigurationManager>();
        _resourcesService = Substitute.For<IResourcesService>();
        _roomsManager = Substitute.For<IRoomsManager>();

        _contextProvider = new DefaultContextProvider(_configurationManager, _resourcesService, _roomsManager);
    }

    [Test]
    public void Test_CurrentWhitelist_ShouldReturnWhitelistFromConfiguration()
    {
        // Arrange
        var expectedWhitelist = new[] { "room1", "room2" };
        _configurationManager.Configuration.Whitelist.Returns(expectedWhitelist);

        // Act
        var result = _contextProvider.CurrentWhitelist;

        // Assert
        Assert.That(result, Is.EqualTo(expectedWhitelist));
    }

    [Test]
    public void Test_DefaultRoom_ShouldReturnDefaultRoomFromConfiguration()
    {
        // Arrange
        var expectedRoom = "mainRoom";
        _configurationManager.Configuration.DefaultRoom.Returns(expectedRoom);

        // Act
        var result = _contextProvider.DefaultRoom;

        // Assert
        Assert.That(result, Is.EqualTo(expectedRoom));
    }

    [Test]
    public void Test_DefaultCulture_ShouldReturnCultureFromConfiguration()
    {
        // Arrange
        var expectedLocale = "fr-FR";
        _configurationManager.Configuration.DefaultLocaleCode.Returns(expectedLocale);

        // Act
        var result = _contextProvider.DefaultCulture;

        // Assert
        Assert.That(result.Name, Is.EqualTo(expectedLocale));
    }

    [Test]
    public void Test_GetString_ShouldReturnLocalizedString_WhenExists()
    {
        // Arrange
        var key = "hello";
        var culture = new CultureInfo("en-US");
        var expectedValue = "Hello";
        _resourcesService.GetString(key, culture).Returns(expectedValue);

        // Act
        var result = _contextProvider.GetString(key, culture);

        // Assert
        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [Test]
    public void Test_GetString_ShouldReturnKey_WhenLocalizedStringIsNull()
    {
        // Arrange
        var key = "missing_key";
        var culture = new CultureInfo("en-US");
        _resourcesService.GetString(key, culture).Returns((string)null);

        // Act
        var result = _contextProvider.GetString(key, culture);

        // Assert
        Assert.That(result, Is.EqualTo(key));
    }

    [Test]
    public void Test_GetString_ShouldReturnKey_WhenLocalizedStringIsWhitespace()
    {
        // Arrange
        var key = "empty_key";
        var culture = new CultureInfo("en-US");
        _resourcesService.GetString(key, culture).Returns("  ");

        // Act
        var result = _contextProvider.GetString(key, culture);

        // Assert
        Assert.That(result, Is.EqualTo(key));
    }

    [Test]
    public void Test_GetRoom_ShouldReturnRoom_WhenExists()
    {
        // Arrange
        var roomId = "room1";
        var expectedRoom = Substitute.For<IRoom>();
        _roomsManager.GetRoom(roomId).Returns(expectedRoom);

        // Act
        var result = _contextProvider.GetRoom(roomId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedRoom));
    }

    [Test]
    public void Test_GetRoom_ShouldReturnNull_WhenRoomDoesNotExist()
    {
        // Arrange
        var roomId = "invalidRoom";
        _roomsManager.GetRoom(roomId).Returns((IRoom)null);

        // Act
        var result = _contextProvider.GetRoom(roomId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_GetRoomParameterValue_ShouldReturnParameterValue_WhenExists()
    {
        // Arrange
        var roomId = "room1";
        var key = "setting";
        var expectedValue = "enabled";
        _roomsManager.GetRoomBotConfigurationParameterValue(roomId, key).Returns(expectedValue);

        // Act
        var result = _contextProvider.GetRoomParameterValue(roomId, key);

        // Assert
        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [Test]
    public void Test_GetRoomParameterValue_ShouldReturnNull_WhenParameterDoesNotExist()
    {
        // Arrange
        var roomId = "room1";
        var key = "invalid_setting";
        _roomsManager.GetRoomBotConfigurationParameterValue(roomId, key).Returns((string)null);

        // Act
        var result = _contextProvider.GetRoomParameterValue(roomId, key);

        // Assert
        Assert.That(result, Is.Null);
    }
}