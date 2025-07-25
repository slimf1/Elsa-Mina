﻿using ElsaMina.Core.Services.Config;
using Newtonsoft.Json;

namespace ElsaMina.UnitTests.Core.Services.Config;

public class ConfigurationServiceTest
{
    private ConfigurationManager _configurationManager;

    [SetUp]
    public void SetUp()
    {
        _configurationManager = new ConfigurationManager();
    }

    [Test]
    public async Task Test_LoadConfigurationAsync_ShouldLoadConfigurationFromReader()
    {
        // Arrange
        var reader = new StringReader("{\"Name\": \"test\", \"Host\": \"test.server.com\"}");

        // Act
        await _configurationManager.LoadConfigurationAsync(reader);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_configurationManager.Configuration, Is.Not.Null);
            Assert.That(_configurationManager.Configuration.Name, Is.EqualTo("test"));
            Assert.That(_configurationManager.Configuration.Host, Is.EqualTo("test.server.com"));
        });
    }

    [Test]
    public void Test_LoadConfigurationAsync_ShouldThrowException_WhenJsonIsInvalid()
    {
        // Arrange
        var reader = new StringReader("{\"Env\": \"test\", \"Host\": \"test.server.com\"");

        // Act & Assert
        Assert.ThrowsAsync<JsonSerializationException>(
            async () => await _configurationManager.LoadConfigurationAsync(reader));
    }
}