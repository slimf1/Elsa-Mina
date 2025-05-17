using ElsaMina.Commands.Ai.Chat;
using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using NSubstitute;

namespace ElsaMina.Test.Commands.Ai.LanguageModel;

[TestFixture]
public class MistralLanguageModelProviderTest
{
    private IHttpService _httpService;
    private IConfiguration _configuration;
    private MistralLanguageModelProvider _languageModelProvider;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _configuration = Substitute.For<IConfiguration>();
        _languageModelProvider = new MistralLanguageModelProvider(_httpService, _configuration);
    }

    [Test]
    public async Task Test_AskLlmAsync_ShouldReturnNull_WhenApiKeyIsMissing()
    {
        // Arrange
        _configuration.MistralApiKey.Returns(string.Empty);

        // Act
        var result = await _languageModelProvider.AskLanguageModelAsync("test prompt");

        // Assert
        Assert.IsNull(result);
        await _httpService.DidNotReceiveWithAnyArgs().PostJsonAsync<MistralRequestDto, MistralResponseDto>(default, default, default, default);
    }

    [Test]
    public async Task Test_AskLlmAsync_ShouldCallHttpService_WithCorrectParameters()
    {
        // Arrange
        const string apiKey = "test-api-key";
        const string prompt = "test prompt";
        const string expectedResponse = "response content";

        _configuration.MistralApiKey.Returns(apiKey);

        var mistralResponse = new MistralResponseDto
        {
            Choices =
            [
                new MistralChoiceDto
                {
                    Message = new MistralResponseMessageDto
                    {
                        Content = expectedResponse
                    }
                }
            ]
        };

        _httpService
            .PostJsonAsync<MistralRequestDto, MistralResponseDto>(
                Arg.Any<string>(),
                Arg.Any<MistralRequestDto>(),
                headers: Arg.Any<IDictionary<string, string>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(new HttpResponse<MistralResponseDto> { Data = mistralResponse });

        // Act
        var result = await _languageModelProvider.AskLanguageModelAsync(prompt);

        // Assert
        Assert.AreEqual(expectedResponse, result);
        await _httpService.Received(1).PostJsonAsync<MistralRequestDto, MistralResponseDto>(
            Arg.Is<string>(url => url == "https://api.mistral.ai/v1/chat/completions"),
            Arg.Is<MistralRequestDto>(dto => dto.Messages[0].Content == prompt),
            headers: Arg.Is<IDictionary<string, string>>(headers => headers["Authorization"] == $"Bearer {apiKey}"),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_AskLlmAsync_ShouldReturnNull_WhenHttpResponseIsNull()
    {
        // Arrange
        _configuration.MistralApiKey.Returns("test-api-key");

        _httpService
            .PostJsonAsync<MistralRequestDto, MistralResponseDto>(
                Arg.Any<string>(),
                Arg.Any<MistralRequestDto>(),
                headers: Arg.Any<IDictionary<string, string>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns((HttpResponse<MistralResponseDto>)null);

        // Act
        var result = await _languageModelProvider.AskLanguageModelAsync("test prompt");

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task Test_AskLlmAsync_ShouldReturnNull_WhenNoChoicesArePresent()
    {
        // Arrange
        _configuration.MistralApiKey.Returns("test-api-key");

        var mistralResponse = new MistralResponseDto { Choices = null };

        _httpService
            .PostJsonAsync<MistralRequestDto, MistralResponseDto>(
                Arg.Any<string>(),
                Arg.Any<MistralRequestDto>(),
                headers: Arg.Any<IDictionary<string, string>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(new HttpResponse<MistralResponseDto> { Data = mistralResponse });

        // Act
        var result = await _languageModelProvider.AskLanguageModelAsync("test prompt");

        // Assert
        Assert.IsNull(result);
    }
}