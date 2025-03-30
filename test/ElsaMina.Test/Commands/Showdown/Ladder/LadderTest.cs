using ElsaMina.Commands.Showdown.Ladder;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Templates;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.Showdown.Ladder;

public class LadderCommandTest
{
    private LadderCommand _ladderCommand;
    private IHttpService _httpService;
    private ITemplatesManager _templatesManager;
    private IFormatsManager _formatsManager;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _formatsManager = Substitute.For<IFormatsManager>();
        _context = Substitute.For<IContext>();
        _ladderCommand = new LadderCommand(_httpService, _templatesManager, _formatsManager);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithNoPlayersMessage_WhenResponseIsNull()
    {
        // Arrange
        _httpService.GetAsync<LadderDto>(Arg.Any<string>()).ReturnsNull();
        _context.Target.Returns("gen8ou");

        // Act
        await _ladderCommand.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("ladder_no_players");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithNoPlayersMessage_WhenTopListIsNull()
    {
        // Arrange
        var httpResponse = new HttpResponse<LadderDto> { Data = new LadderDto { TopList = null, Format = "gen8ou" } };
        _httpService.GetAsync<LadderDto>(Arg.Any<string>()).Returns(httpResponse);
        _context.Target.Returns("gen8ou");

        // Act
        await _ladderCommand.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("ladder_no_players");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithNoPlayersMessage_WhenNoPlayersMatchPrefix()
    {
        // 
        var httpResponse = new HttpResponse<LadderDto>
        {
            Data = new LadderDto
                { TopList = new List<LadderPlayerDto> { new() { Username = "player1" } }, Format = "gen8ou" }
        };
        _httpService.GetAsync<LadderDto>(Arg.Any<string>()).Returns(httpResponse);
        _context.Target.Returns("gen8ou,xyz");

        // Act
        await _ladderCommand.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("ladder_no_players");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendHtml_WhenPlayersMatchPrefix()
    {
        // Arrange
        var player = new LadderPlayerDto { Username = "player1" };
        var httpResponse = new HttpResponse<LadderDto>
            { Data = new LadderDto { TopList = new List<LadderPlayerDto> { player }, Format = "gen8ou" } };
        _httpService.GetAsync<LadderDto>(Arg.Any<string>()).Returns(httpResponse);
        _formatsManager.GetCleanFormat("gen8ou").Returns("[Gen 8] OU");
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<LadderTableViewModel>())
            .Returns(Task.FromResult("formatted_html"));
        _context.Target.Returns("gen8ou,pla");

        // Act
        await _ladderCommand.RunAsync(_context);

        // Assert
        await _templatesManager.Received().GetTemplateAsync("Showdown/Ladder/LadderTable",
            Arg.Is<LadderTableViewModel>(vm => vm.Format == "[Gen 8] OU"));
        _context.Received().SendHtml("formatted_html", rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithErrorMessage_WhenExceptionOccurs()
    {
        // Arrange
        _httpService.GetAsync<LadderDto>(Arg.Any<string>())
            .Throws(new Exception("API failure"));
        _context.Target.Returns("gen8ou");

        // Act
        await _ladderCommand.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("ladder_error");
    }
}