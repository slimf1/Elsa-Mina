using ElsaMina.Commands.AiTts;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Http;
using ElsaMina.FileSharing;
using NSubstitute;

namespace ElsaMina.Test.Commands.AiTts;

public class SpeakCommandTest
{
    private IHttpService _httpService;
    private IConfiguration _configuration;
    private IFileSharingService _fileSharingService;
    private IClockService _clockService;
    private IContext _context;

    private SpeakCommand _command;
    
    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _fileSharingService = Substitute.For<IFileSharingService>();
        _clockService = Substitute.For<IClockService>();
        _configuration = Substitute.For<IConfiguration>();
        
        _context = Substitute.For<IContext>();
        
        _command = new SpeakCommand(_httpService,
            _fileSharingService, _clockService, _configuration);
    }
    
    [Test]
    [TestCase(null)]
    [TestCase("")]
    public async Task Test_RunAsync_ShouldDoNothing_WhenApiKeyIsMissing(string key)
    {
        // Given
        _configuration.ElevenLabsApiKey.Returns(key);
        
        // When
        await _command.RunAsync(_context);
        
        // Then
        await _httpService.DidNotReceive().PostStreamAsync(Arg.Any<string>(), Arg.Any<Stream>(),
            Arg.Any<IDictionary<string, string>>());
        await _fileSharingService.DidNotReceive().CreateFileAsync(Arg.Any<Stream>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
    
    [Test]
    public async Task Test_RunAsync_ShouldFetchStreamAndUploadIt_WhenArgsAreValid()
    {
        // Given
        _configuration.ElevenLabsApiKey.Returns("key");
        _context.Target.Returns("target");
        var stream = new MemoryStream([1, 2, 3]);
        _clockService.CurrentUtcDateTime.Returns(new DateTime(2020, 5, 3, 10, 30, 45, 300));
        _httpService.PostStreamAsync(Arg.Any<string>(), Arg.Is<ElevenLabsRequestDto>(dto => dto.Text == "target"),
            Arg.Any<IDictionary<string, string>>()).Returns(stream);
        _fileSharingService.CreateFileAsync(stream, "speakcmd_20200503_103045300.mp3",
                "Speak command", "audio/mpeg")
            .Returns("url");

        
        // When
        await _command.RunAsync(_context);
        
        // Then
        _context.Received(1).SendHtml("""<audio src="url" controls></audio>""");
    }
    
    [Test]
    public async Task Test_RunAsync_ShouldFetchStreamAndUploadIt()
    {
        // Given
        _configuration.ElevenLabsApiKey.Returns("key");
        _context.Target.Returns("target");
        var stream = new MemoryStream([1, 2, 3]);
        _clockService.CurrentUtcDateTime.Returns(new DateTime(2020, 5, 3, 10, 30, 45, 300));
        _httpService.PostStreamAsync(Arg.Any<string>(), Arg.Is<ElevenLabsRequestDto>(dto => dto.Text == "target"),
            Arg.Any<IDictionary<string, string>>()).Returns(stream);
        _fileSharingService.CreateFileAsync(stream, "speakcmd_20200503_103045300.mp3",
                "Speak command", "audio/mpeg")
            .Returns("url");

        // When
        await _command.RunAsync(_context);
        
        // Then
        _context.Received(1).SendHtml("""<audio src="url" controls></audio>""");
    }
}