using ElsaMina.Commands.AiChat;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.AiChat;

public class AskElsaCommandTest
{
    private IHttpService _httpService;
    private IConfigurationManager _configurationManager;
    private IRoomsManager _roomsManager;
    private AskElsaCommand _command;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _command = new AskElsaCommand(_httpService, _configurationManager, _roomsManager);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver_WhenCreated()
    {
        Assert.That(Rank.Voiced, Is.EqualTo(_command.RequiredRank));
    }

    [Test]
    public async Task Test_Run_ShouldLogError_WhenApiKeyIsMissing()
    {
        // Arrange
        _configurationManager.Configuration.MistralApiKey.Returns(string.Empty);
        var context = Substitute.For<IContext>();

        // Act
        await _command.Run(context);

        // Assert
        context.DidNotReceive().Reply(Arg.Any<string>());
    }

    [Test]
    public async Task Test_Run_ShouldSendRequestWithCorrectHeaders_WhenApiKeyIsProvided()
    {
        // Arrange
        const string apiKey = "test-api-key";
        const string roomId = "room-1";
        const string roomName = "RoomName";

        _configurationManager.Configuration.MistralApiKey.Returns(apiKey);
        _configurationManager.Configuration.Name.Returns("Elsa");

        var context = Substitute.For<IContext>();
        context.RoomId.Returns(roomId);
        context.Target.Returns("Bonjour");
        context.Sender.Name.Returns("User123");

        var room = Substitute.For<IRoom>();
        room.Name.Returns(roomName);
        room.LastMessages.Returns([Tuple.Create("Alice", "Hello"), Tuple.Create("Bob", "Hi")]);

        _roomsManager.GetRoom(roomId).Returns(room);

        var response = new MistralResponseDto
        {
            Choices = [new MistralChoiceDto { Message = new MistralResponseMessageDto { Content = "Hello" } }]
        };
        _httpService
            .PostJson<MistralRequestDto, MistralResponseDto>(
                Arg.Any<string>(),
                Arg.Any<MistralRequestDto>(),
                headers: Arg.Any<Dictionary<string, string>>())
            .Returns(new HttpResponse<MistralResponseDto> { Data = response });

        // Act
        await _command.Run(context);

        // Assert
        await _httpService.Received(1).PostJson<MistralRequestDto, MistralResponseDto>(
            "https://api.mistral.ai/v1/chat/completions",
            Arg.Is<MistralRequestDto>(dto =>
                dto.Model == "mistral-large-latest" &&
                dto.Messages.First().Content.Contains("Bonjour") &&
                dto.Messages.First().Content.Contains("User123") &&
                dto.Messages.First().Content.Contains("Alice: Hello, Bob: Hi")
            ),
            headers: Arg.Is<Dictionary<string, string>>(headers => headers["Authorization"] == $"Bearer {apiKey}")
        );
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithErrorMessage_WhenResponseChoiceIsNull()
    {
        // Arrange
        _configurationManager.Configuration.MistralApiKey.Returns("valid-api-key");
        var context = Substitute.For<IContext>();

        _httpService
            .PostJson<MistralRequestDto, MistralResponseDto>(
                Arg.Any<string>(),
                Arg.Any<MistralRequestDto>(),
                headers: Arg.Any<Dictionary<string, string>>())
            .ReturnsNull();

        // Act
        await _command.Run(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("ask_error");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithResponseContent_WhenResponseChoiceIsNotNull()
    {
        // Arrange
        _configurationManager.Configuration.MistralApiKey.Returns("valid-api-key");
        var context = Substitute.For<IContext>();
        var response = new MistralResponseDto
        {
            Choices =
            [
                new MistralChoiceDto { Message = new MistralResponseMessageDto { Content = "Hello from Mistral" } }
            ]
        };
        _httpService
            .PostJson<MistralRequestDto, MistralResponseDto>(
                Arg.Any<string>(),
                Arg.Any<MistralRequestDto>(),
                headers: Arg.Any<Dictionary<string, string>>())
            .Returns(new HttpResponse<MistralResponseDto> { Data = response });

        // Act
        await _command.Run(context);

        // Assert
        context.Received(1).Reply("Hello from Mistral");
    }
}