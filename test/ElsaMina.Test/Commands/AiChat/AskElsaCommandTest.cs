using System.Globalization;
using ElsaMina.Commands.AiChat;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.AiChat;

public class AskElsaCommandTest
{
    private IHttpService _httpService;
    private IConfiguration _configuration;
    private IResourcesService _resourcesService;
    private AskElsaCommand _command;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _configuration = Substitute.For<IConfiguration>();
        _resourcesService = Substitute.For<IResourcesService>();
        _command = new AskElsaCommand(_httpService, _configuration, _resourcesService);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver_WhenCreated()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public async Task Test_RunAsync_ShouldLogError_WhenApiKeyIsMissing()
    {
        // Arrange
        _configuration.MistralApiKey.Returns(string.Empty);
        var context = Substitute.For<IContext>();

        // Act
        await _command.RunAsync(context);

        // Assert
        context.DidNotReceive().Reply(Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendRequestWithCorrectHeaders_WhenApiKeyIsProvided()
    {
        // Arrange
        const string apiKey = "test-api-key";
        const string roomId = "room-1";
        const string roomName = "RoomName";
        const string prompt = "test-prompt {0} {1} {2} {3} {4}";

        _configuration.MistralApiKey.Returns(apiKey);
        _configuration.Name.Returns("Elsa");

        var context = Substitute.For<IContext>();
        context.RoomId.Returns(roomId);
        context.Target.Returns("Bonjour");
        context.Sender.Name.Returns("User123");
        _resourcesService.GetString("ask_prompt", Arg.Any<CultureInfo>()).Returns(prompt);

        var room = Substitute.For<IRoom>();
        room.Name.Returns(roomName);
        room.LastMessages.Returns([Tuple.Create("Alice", "Hello"), Tuple.Create("Bob", "Hi")]);
        context.Room.Returns(room);

        var response = new MistralResponseDto
        {
            Choices = [new MistralChoiceDto { Message = new MistralResponseMessageDto { Content = "Hello" } }]
        };
        _httpService
            .PostJsonAsync<MistralRequestDto, MistralResponseDto>(
                Arg.Any<string>(),
                Arg.Any<MistralRequestDto>(),
                headers: Arg.Any<Dictionary<string, string>>())
            .Returns(new HttpResponse<MistralResponseDto> { Data = response });

        // Act
        await _command.RunAsync(context);

        // Assert
        await _httpService.Received(1).PostJsonAsync<MistralRequestDto, MistralResponseDto>(
            "https://api.mistral.ai/v1/chat/completions",
            Arg.Is<MistralRequestDto>(dto =>
                dto.Model == "mistral-large-latest" &&
                dto.Messages.First().Content.Contains("test-prompt") &&
                dto.Messages.First().Content.Contains("Bonjour") &&
                dto.Messages.First().Content.Contains("User123") &&
                dto.Messages.First().Content.Contains("Alice: Hello, Bob: Hi")
            ),
            headers: Arg.Is<Dictionary<string, string>>(headers => headers["Authorization"] == $"Bearer {apiKey}")
        );
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithErrorMessage_WhenResponseChoiceIsNull()
    {
        // Arrange
        _configuration.MistralApiKey.Returns("valid-api-key");
        var context = Substitute.For<IContext>();

        _httpService
            .PostJsonAsync<MistralRequestDto, MistralResponseDto>(
                Arg.Any<string>(),
                Arg.Any<MistralRequestDto>(),
                headers: Arg.Any<Dictionary<string, string>>())
            .ReturnsNull();

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("ask_error");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithResponseContent_WhenResponseChoiceIsNotNull()
    {
        // Arrange
        _configuration.MistralApiKey.Returns("valid-api-key");
        var context = Substitute.For<IContext>();
        var response = new MistralResponseDto
        {
            Choices =
            [
                new MistralChoiceDto { Message = new MistralResponseMessageDto { Content = "Hello from Mistral" } }
            ]
        };
        _httpService
            .PostJsonAsync<MistralRequestDto, MistralResponseDto>(
                Arg.Any<string>(),
                Arg.Any<MistralRequestDto>(),
                headers: Arg.Any<Dictionary<string, string>>())
            .Returns(new HttpResponse<MistralResponseDto> { Data = response });

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).Reply("Hello from Mistral");
    }
}