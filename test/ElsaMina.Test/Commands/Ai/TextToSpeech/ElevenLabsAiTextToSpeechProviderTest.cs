using ElsaMina.Commands.Ai.TextToSpeech;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using NSubstitute;

namespace ElsaMina.Test.Commands.Ai.TextToSpeech;

public class ElevenLabsAiTextToSpeechProviderTest
{
    private IConfiguration _mockConfiguration;
    private IHttpService _mockHttpService;
    private ElevenLabsAiTextToSpeechProvider _provider;
    
    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockHttpService = Substitute.For<IHttpService>();
        _provider = new ElevenLabsAiTextToSpeechProvider(_mockConfiguration, _mockHttpService);
    }
    
    [Test]
    public async Task Test_GetTextToSpeechAudioStreamAsync_ReturnsNull_WhenApiKeyIsMissing()
    {
        // Arrange
        _mockConfiguration.ElevenLabsApiKey.Returns(string.Empty);
    
        // Act
        var result = await _provider.GetTextToSpeechAudioStreamAsync("Test text");
    
        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task Test_GetTextToSpeechAudioStreamAsync_ReturnsStream_WhenApiKeyIsValid()
    {
        // Arrange
        var mockStream = new MemoryStream();
        _mockConfiguration.ElevenLabsApiKey.Returns("valid-api-key");
        _mockHttpService.PostStreamAsync(
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<Dictionary<string, string>>(),
            Arg.Any<CancellationToken>()).Returns(mockStream);
    
        // Act
        var result = await _provider.GetTextToSpeechAudioStreamAsync("Test text");
    
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(mockStream));
    }
    
    [Test]
    public async Task Test_GetTextToSpeechAudioStreamAsync_CallsHttpServiceWithCorrectParameters()
    {
        // Arrange
        var mockStream = new MemoryStream();
        _mockConfiguration.ElevenLabsApiKey.Returns("valid-api-key");
        _mockHttpService.PostStreamAsync(
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<Dictionary<string, string>>(),
            Arg.Any<CancellationToken>()).Returns(mockStream);
        var cancellationToken = new CancellationToken();
        const string text = "Test text";
    
        // Act
        await _provider.GetTextToSpeechAudioStreamAsync(text, cancellationToken);
    
        // Assert
        await _mockHttpService.Received(1).PostStreamAsync(
            "https://api.elevenlabs.io/v1/text-to-speech/Qrl71rx6Yg8RvyPYRGCQ",
            Arg.Is<ElevenLabsRequestDto>(dto => dto.Text == text && dto.ModelId == "eleven_multilingual_v1"),
            Arg.Is<Dictionary<string, string>>(headers => headers["xi-api-key"] == "valid-api-key"),
            cancellationToken);
    }
}