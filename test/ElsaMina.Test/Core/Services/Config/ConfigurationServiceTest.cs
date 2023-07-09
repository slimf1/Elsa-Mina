using ElsaMina.Core.Services.Config;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Serilog;

namespace ElsaMina.Test.Core.Services.Config;

public class ConfigurationServiceTest
{
    private ILogger _logger;
    private ConfigurationManager _configurationManager;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
        
        _configurationManager = new ConfigurationManager(_logger);
    }

    [Test]
    public async Task Test_LoadConfiguration_ShouldLoadConfigurationFromReader()
    {
        // Arrange
        var reader = new StringReader("{\"Env\": \"test\", \"Host\": \"test.server.com\"}");

        // Act
        await _configurationManager.LoadConfiguration(reader);

        // Assert
        _configurationManager.Configuration.Should().NotBeNull();
        _configurationManager.Configuration.Env.Should().Be("test");
        _configurationManager.Configuration.Host.Should().Be("test.server.com");
    }

    [Test]
    public async Task Test_LoadConfiguration_ShouldThrowException_WhenJsonIsInvalid()
    {
        // Arrange
        var reader = new StringReader("{\"Env\": \"test\", \"Host\": \"test.server.com\"");

        // Act
        Func<Task> action = async () => await _configurationManager.LoadConfiguration(reader);

        // Assert
        await action.Should().ThrowAsync<JsonSerializationException>();
    }
}