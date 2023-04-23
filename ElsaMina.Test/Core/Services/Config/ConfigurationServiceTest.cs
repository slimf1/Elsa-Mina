using ElsaMina.Core.Services.Config;
using FluentAssertions;
using Newtonsoft.Json;

namespace ElsaMina.Test.Core.Services.Config;

public class ConfigurationServiceTest
{
    private ConfigurationService _configurationService;

    [SetUp]
    public void SetUp()
    {
        _configurationService = new ConfigurationService();
    }

    [Test]
    public async Task Test_LoadConfiguration_ShouldLoadConfigurationFromReader()
    {
        // Arrange
        var reader = new StringReader("{\"Env\": \"test\", \"Host\": \"test.server.com\"}");

        // Act
        await _configurationService.LoadConfiguration(reader);

        // Assert
        _configurationService.Configuration.Should().NotBeNull();
        _configurationService.Configuration!.Env.Should().Be("test");
        _configurationService.Configuration!.Host.Should().Be("test.server.com");
    }

    [Test]
    public async Task Test_LoadConfiguration_ShouldThrowException_WhenJsonIsInvalid()
    {
        // Arrange
        var reader = new StringReader("{\"Env\": \"test\", \"Host\": \"test.server.com\"");

        // Act
        Func<Task> action = async () => await _configurationService.LoadConfiguration(reader);

        // Assert
        await action.Should().ThrowAsync<JsonSerializationException>();
    }
}