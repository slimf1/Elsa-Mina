using System.Globalization;
using System.Net;
using ElsaMina.Commands.Misc.Wiki;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Images;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Misc.Wiki;

public class WikipediaSearchCommandTest
{
    private IHttpService _mockHttpService;
    private IImageService _imageService;
    private WikipediaSearchCommand _command;

    [SetUp]
    public void SetUp()
    {
        _mockHttpService = Substitute.For<IHttpService>();
        _imageService = Substitute.For<IImageService>();
        _command = new WikipediaSearchCommand(_mockHttpService, _imageService);
    }

    [Test]
    public async Task RunAsync_ShouldReplyWithResults_WhenSearchReturnsResults()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Culture.Returns(new CultureInfo("fr-FR"));
        context.Target.Returns("Test");
        var searchResponse = new WikipediaApiSearchResponse
        {
            Query = new QueryPages
            {
                Pages = new Dictionary<string, WikiPage>
                {
                    { "1", new WikiPage { PageId = 1, Title = "Test Page" } }
                }
            }
        };
        var extractResponse = new WikipediaExtractResponse
        {
            Query = new QueryWithExtract
            {
                Pages = new Dictionary<string, WikiExtractPage>
                {
                    { "1", new WikiExtractPage { PageId = 1, Title = "Test Page", Extract = "Summary line." } }
                }
            }
        };
        _mockHttpService.GetAsync<WikipediaApiSearchResponse>(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>(), Arg.Any<IDictionary<string, string>>(), false, isRaw: false, Arg.Any<CancellationToken>())
            .Returns(new HttpResponse<WikipediaApiSearchResponse> { Data = searchResponse });
        _mockHttpService.GetAsync<WikipediaExtractResponse>(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>(), Arg.Any<IDictionary<string, string>>(), false, isRaw: false, Arg.Any<CancellationToken>())
            .Returns(new HttpResponse<WikipediaExtractResponse> { Data = extractResponse });

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).SendHtmlIn(Arg.Is<string>(s => s.Contains("Test Page") && s.Contains("Summary line.")), rankAware: true);
    }

    [Test]
    public async Task RunAsync_ShouldReplyWithNoResultsMessage_WhenSearchReturnsEmptyResults()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Culture.Returns(new CultureInfo("fr-FR"));
        context.Target.Returns("Test");
        var searchResponse = new WikipediaApiSearchResponse
        {
            Query = new QueryPages
            {
                Pages = new Dictionary<string, WikiPage>()
            }
        };
        _mockHttpService.GetAsync<WikipediaApiSearchResponse>(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>(), Arg.Any<IDictionary<string, string>>(), false, isRaw: true, Arg.Any<CancellationToken>())
            .Returns(new HttpResponse<WikipediaApiSearchResponse> { Data = searchResponse });

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).ReplyRankAwareLocalizedMessage("wiki_page_not_found");
    }

    [Test]
    public async Task RunAsync_ShouldReplyWithErrorMessage_WhenHttpRequestFails()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Culture.Returns(new CultureInfo("fr-FR"));
        context.Target.Returns("Test");
        _mockHttpService.GetAsync<WikipediaApiSearchResponse>(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>(), Arg.Any<IDictionary<string, string>>(), false, isRaw: false, Arg.Any<CancellationToken>())
            .Throws(new Exception("Error"));

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).ReplyRankAwareLocalizedMessage("wiki_error", "Error");
    }
}