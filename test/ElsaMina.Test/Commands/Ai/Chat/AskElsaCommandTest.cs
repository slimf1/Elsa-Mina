using System.Globalization;
using ElsaMina.Commands.Ai.Chat;
using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.Test.Commands.Ai.Chat;

public class AskElsaCommandTests
{
    private IConfiguration _configuration;
    private IResourcesService _resourcesService;
    private ILanguageModelProvider _languageModelProvider;
    private AskElsaCommand _command;

    [SetUp]
    public void SetUp()
    {
        _configuration = Substitute.For<IConfiguration>();
        _resourcesService = Substitute.For<IResourcesService>();
        _languageModelProvider = Substitute.For<ILanguageModelProvider>();
        _command = new AskElsaCommand(_configuration, _resourcesService, _languageModelProvider);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeVoiced()
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
    public async Task Test_RunAsync_ShouldGeneratePromptCorrectly()
    {
        // Arrange
        const string apiKey = "test-api-key";
        const string roomName = "TestRoom";
        const string senderName = "User123";
        const string botName = "Elsa";
        const string target = "Hello";
        const string promptTemplate = "Prompt: {0}, {1}, {2}, {3}, {4}";
        const string expectedPrompt = "Prompt: Hello, User123, Elsa, TestRoom, Alice: Hi, Bob: Hello";

        _configuration.MistralApiKey.Returns(apiKey);
        _configuration.Name.Returns(botName);
        _resourcesService.GetString("ask_prompt", Arg.Any<CultureInfo>()).Returns(promptTemplate);

        var context = Substitute.For<IContext>();
        context.Target.Returns(target);
        context.Sender.Name.Returns(senderName);
        context.Room.Name.Returns(roomName);
        context.Room.LastMessages.Returns(new[] { Tuple.Create("Alice", "Hi"), Tuple.Create("Bob", "Hello") });

        // Act
        await _command.RunAsync(context);

        // Assert
        await _languageModelProvider.Received(1).AskLanguageModelAsync(expectedPrompt, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithErrorMessage_WhenResponseIsNull()
    {
        // Arrange
        _configuration.MistralApiKey.Returns("valid-api-key");
        var context = Substitute.For<IContext>();
        _languageModelProvider.AskLanguageModelAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((string)null);

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("ask_error");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithResponse_WhenResponseIsNotNull()
    {
        // Arrange
        _configuration.MistralApiKey.Returns("valid-api-key");
        var context = Substitute.For<IContext>();
        const string response = "Hello from AI";

        _languageModelProvider.AskLanguageModelAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(response);

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).Reply(response);
    }
}