using System.Globalization;
using ElsaMina.Commands.Ai.Chat;
using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Commands.Ai.TextToSpeech;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Ai.Chat;

public class AskElsaCommandTest
{
    private IConfiguration _mockConfiguration;
    private IResourcesService _mockResourcesService;
    private ILanguageModelProvider _mockLanguageModelProvider;
    private IAiTextToSpeechProvider _mockTextToSpeechProvider;
    private IConversationHistoryService _mockConversationHistory;
    private AskElsaCommand _command;

    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockResourcesService = Substitute.For<IResourcesService>();
        _mockLanguageModelProvider = Substitute.For<ILanguageModelProvider>();
        _mockTextToSpeechProvider = Substitute.For<IAiTextToSpeechProvider>();
        _mockConversationHistory = Substitute.For<IConversationHistoryService>();
        _mockConversationHistory.BuildConversation(Arg.Any<IRoom>(), Arg.Any<IUser>(), Arg.Any<string>())
            .Returns(new List<LanguageModelMessage>());

        _command = new AskElsaCommand(
            _mockConfiguration,
            _mockResourcesService,
            _mockLanguageModelProvider,
            _mockTextToSpeechProvider,
            _mockConversationHistory
        );
    }
    
    [Test]
    public void Test_RequiredRank_ShouldBeVoiced()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public async Task RunAsync_ShouldReplyWithText_WhenAudioIsNotRequested()
    {
        // Arrange
        var mockContext = Substitute.For<IContext>();
        mockContext.Command.Returns("ask");
        mockContext.Target.Returns("What is the weather?");
        mockContext.Sender.Name.Returns("User");
        mockContext.Room.Name.Returns("Room1");
        mockContext.Room.LastMessages.Returns(new List<Tuple<string, string>>());
        _mockResourcesService.GetString("ask_prompt", Arg.Any<CultureInfo>())
            .Returns("{0} asked by {1} in {3}");
        _mockLanguageModelProvider.AskLanguageModelAsync(Arg.Any<LanguageModelRequest>(), Arg.Any<CancellationToken>())
            .Returns("It's sunny.");

        // Act
        await _command.RunAsync(mockContext);

        // Assert
        mockContext.Received(1).Reply("It's sunny.");
    }

    [Test]
    public async Task RunAsync_ShouldReplyWithAudio_WhenAudioIsRequested()
    {
        // Arrange
        var mockContext = Substitute.For<IContext>();
        mockContext.Command.Returns("askaudio");
        mockContext.Target.Returns("What is the weather?");
        mockContext.Sender.Name.Returns("User");
        mockContext.Room.Name.Returns("Room1");
        mockContext.Room.LastMessages.Returns(new List<Tuple<string, string>>());
        _mockResourcesService.GetString("ask_prompt", Arg.Any<CultureInfo>())
            .Returns("{0} asked by {1} in {3}");
        _mockLanguageModelProvider.AskLanguageModelAsync(Arg.Any<LanguageModelRequest>(), Arg.Any<CancellationToken>())
            .Returns("It's sunny.");
        _mockTextToSpeechProvider.GetTextToSpeechAudioUrlAsync(Arg.Any<string>(), Arg.Any<VoiceType>(), Arg.Any<CancellationToken>())
            .Returns("https://example.com/audio.mp3");

        // Act
        await _command.RunAsync(mockContext);

        // Assert
        mockContext.Received(1).ReplyHtml("""<audio src="https://example.com/audio.mp3" controls aria-label="It's sunny."></audio>""");
    }

    [Test]
    public async Task RunAsync_ShouldLogError_WhenAudioGenerationFails()
    {
        // Arrange
        var mockContext = Substitute.For<IContext>();
        mockContext.Command.Returns("askaudio");
        mockContext.Target.Returns("What is the weather?");
        mockContext.Sender.Name.Returns("User");
        mockContext.Room.Name.Returns("Room1");
        mockContext.Room.LastMessages.Returns(new List<Tuple<string, string>>());
        _mockResourcesService.GetString("ask_prompt", Arg.Any<CultureInfo>())
            .Returns("{0} asked by {1} in {3}");
        _mockLanguageModelProvider.AskLanguageModelAsync(Arg.Any<LanguageModelRequest>(), Arg.Any<CancellationToken>())
            .Returns("It's sunny.");
        _mockTextToSpeechProvider.GetTextToSpeechAudioUrlAsync(Arg.Any<string>(), Arg.Any<VoiceType>(), Arg.Any<CancellationToken>())
            .Returns((string)null);

        // Act
        await _command.RunAsync(mockContext);

        // Assert
        mockContext.Received(1).ReplyLocalizedMessage("ask_error");
    }
}
