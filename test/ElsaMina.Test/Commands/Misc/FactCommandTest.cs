using ElsaMina.Commands.Misc.Facts;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.Test.Commands.Misc;

public class FactsCommandTest
{
    private IHttpService _httpService;
    private FactsCommand _factsCommand;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _context = Substitute.For<IContext>();

        _factsCommand = new FactsCommand(_httpService);
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldReturnTrue()
    {
        // Assert
        Assert.That(_factsCommand.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithFact_WhenApiCallSucceeds()
    {
        // Arrange
        var mockResponse = new HttpResponse<FactDto>
        {
            Data = new FactDto
            {
                Text = "Bananas are berries, but strawberries are not!"
            }
        };
        _httpService.Get<FactDto>(Arg.Any<string>()).Returns(mockResponse);
        _context.Command.Returns("fact");

        // Act
        await _factsCommand.Run(_context);

        // Assert
        _context.Received().Reply("**Fact**: Bananas are berries, but strawberries are not!", rankAware: true);
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithGermanFact_WhenCommandIsFactDe()
    {
        // Arrange
        var mockResponse = new HttpResponse<FactDto>
        {
            Data = new FactDto
            {
                Text = "Die Banane ist eine Beere, die Erdbeere jedoch nicht."
            }
        };
        _httpService.Get<FactDto>(Arg.Any<string>()).Returns(mockResponse);
        _context.Command.Returns("factde");

        // Act
        await _factsCommand.Run(_context);

        // Assert
        _context.Received().Reply("**Fact**: Die Banane ist eine Beere, die Erdbeere jedoch nicht.", rankAware: true);
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithError_WhenApiCallFails()
    {
        // Arrange
        _httpService.Get<FactDto>(Arg.Any<string>())
            .Throws(new Exception("API error"));

        // Act
        await _factsCommand.Run(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("fact_error");
    }
}
