using System.Globalization;
using ElsaMina.Commands.Misc.Youtube;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Templates;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.Test.Commands.Misc.Youtube;

public class YoutubeCommandTest
{
    private IHttpService _httpService;
    private IConfigurationManager _configurationManager;
    private ITemplatesManager _templatesManager;
    private YoutubeCommand _youtubeCommand;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _context = Substitute.For<IContext>();

        _youtubeCommand = new YoutubeCommand(_httpService, _configurationManager, _templatesManager);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    public async Task Test_RunAsync_ShouldReturnError_WhenApiKeyIsEmpty(string emptyApiKey)
    {
        // Arrange
        _configurationManager.Configuration.Returns(new Configuration
        {
            YoutubeApiKey = emptyApiKey
        });

        // Act
        await _youtubeCommand.RunAsync(_context);

        // Assert
        await _httpService.DidNotReceive()
            .GetAsync<YouTubeSearchResponse>(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReturnError_WhenNoResultsAreFound()
    {
        // Arrange
        _configurationManager.Configuration.Returns(new Configuration
        {
            YoutubeApiKey = "fakeApiKey"
        });
        _httpService.GetAsync<YouTubeSearchResponse>(Arg.Any<string>(), Arg.Any<Dictionary<string, string>>())
            .Returns(new HttpResponse<YouTubeSearchResponse>
            {
                Data = new YouTubeSearchResponse
                {
                    Items = []
                }
            });

        // Act
        await _youtubeCommand.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("youtube_no_results");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendHtmlResponse_WhenResultsAreFound()
    {
        // Arrange
        _configurationManager.Configuration.Returns(new Configuration
        {
            YoutubeApiKey = "fakeApiKey"
        });
        var mockResponse = new HttpResponse<YouTubeSearchResponse>
        {
            Data = new YouTubeSearchResponse
            {
                Items =
                [
                    new SearchResultItem
                    {
                        Snippet = new Snippet
                        {
                            ChannelTitle = "Test Channel",
                            Description = "Test Description",
                            Title = "Test Video",
                            PublishTime = "2020-05-15T08:38:58Z",
                            Thumbnails = new Thumbnails
                            {
                                Medium = new Thumbnail { Url = "https://test.com/thumbnail.jpg" }
                            }
                        },
                        Id = new VideoId { VideoIdValue = "testVideoId" }
                    }
                ]
            }
        };

        _httpService.GetAsync<YouTubeSearchResponse>(Arg.Any<string>(), Arg.Any<Dictionary<string, string>>())
            .Returns(mockResponse);

        // Act
        await _youtubeCommand.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync("Misc/Youtube/YoutubeVideoPreview",
            Arg.Is<YoutubeVideoPreviewViewModel>(
                vm => vm.VideoId == "testVideoId"
                      && vm.ChannelTitle == "Test Channel"
                      && vm.Description == "Test Description"
                      && vm.Title == "Test Video"
                      && vm.PublishTime == new DateTime(2020, 5, 15, 8, 38, 58, DateTimeKind.Utc)
                      && vm.ThumbnailSource == "https://test.com/thumbnail.jpg"
            ));
    }

    [Test]
    public async Task Test_RunAsync_ShouldLogError_WhenExceptionOccurs()
    {
        // Arrange
        _configurationManager.Configuration.Returns(new Configuration
        {
            YoutubeApiKey = "fakeApiKey"
        });
        _httpService.GetAsync<YouTubeSearchResponse>(Arg.Any<string>(), Arg.Any<Dictionary<string, string>>())
            .Throws(new Exception("Test exception"));

        // Act
        await _youtubeCommand.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("youtube_error_occurred");
    }
}