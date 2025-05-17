using ElsaMina.Commands.Ai.TextToSpeech;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.FileSharing;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.Ai.TextToSpeech;

public class SpeakCommandTest
{
    private IFileSharingService _fileSharingService;
    private IClockService _clockService;
    private IAiTextToSpeechProvider _textToSpeechProvider;
    private IContext _context;

    private SpeakCommand _command;

    [SetUp]
    public void SetUp()
    {
        _fileSharingService = Substitute.For<IFileSharingService>();
        _clockService = Substitute.For<IClockService>();
        _textToSpeechProvider = Substitute.For<IAiTextToSpeechProvider>();
        _context = Substitute.For<IContext>();
        
        _command = new SpeakCommand(_fileSharingService, _clockService, _textToSpeechProvider);
    }
    
    [Test]
    [TestCase(null)]
    [TestCase("")]
    public async Task Test_RunAsync_ShouldReturnError_WhenTextToSpeechFailed(string key)
    {
        // Given
        _textToSpeechProvider
            .GetTextToSpeechAudioStreamAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ReturnsNull();
        
        // When
        await _command.RunAsync(_context);
        
        // Then
        _context.Received(1).ReplyLocalizedMessage("speak_error");
    }
    
    [Test]
    [TestCase(null)]
    [TestCase("")]
    public async Task Test_RunAsync_ShouldReturnError_WhenFileUploadFailed(string key)
    {
        // Given
        _textToSpeechProvider
            .GetTextToSpeechAudioStreamAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        _clockService.CurrentUtcDateTime.Returns(new DateTime(2020, 5, 3, 10, 30, 45, 300));
        _fileSharingService
            .CreateFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .ReturnsNull();
        
        // When
        await _command.RunAsync(_context);
        
        // Then
        _context.Received(1).ReplyLocalizedMessage("speak_error");
    }
    
    [Test]
    public async Task Test_RunAsync_ShouldFetchStreamAndUploadIt_WhenTtsAndFileUploadSucceeds()
    {
        // Given
        _textToSpeechProvider
            .GetTextToSpeechAudioStreamAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        _clockService.CurrentUtcDateTime.Returns(new DateTime(2020, 5, 3, 10, 30, 45, 300));
        _fileSharingService
            .CreateFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .ReturnsNull();
        _fileSharingService.CreateFileAsync(Arg.Any<Stream>(), "speakcmd_20200503_103045300.mp3",
                "Speak command", "audio/mpeg")
            .Returns("url");

        // When
        await _command.RunAsync(_context);
        
        // Then
        _context.Received(1).ReplyHtml("""<audio src="url" controls></audio>""");
    }
}