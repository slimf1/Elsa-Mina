using ElsaMina.Commands.Replays;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.Test.Commands.Replays;

public class ReplaysHandlerTest
{
    private ReplaysHandler _replaysHandler;
    private IHttpService _httpService;
    private ITemplatesManager _templatesManager;
    private IRoomsManager _roomsManager;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        var contextFactory = Substitute.For<IContextFactory>();
        _httpService = Substitute.For<IHttpService>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _context = Substitute.For<IContext>();

        _replaysHandler = new ReplaysHandler(contextFactory, _httpService, _templatesManager, _roomsManager);
    }

    [Test]
    public async Task Test_HandleMessage_ShouldNotProcess_WhenReplayPreviewDisabled()
    {
        // Arrange
        _roomsManager.GetRoomBotConfigurationParameterValue(Arg.Any<string>(), Arg.Any<string>())
            .Returns("false");

        // Act
        await _replaysHandler.HandleMessage(_context);

        // Assert
        await _httpService.DidNotReceiveWithAnyArgs().GetAsync<ReplayDto>(default);
    }

    [Test]
    public async Task Test_HandleMessage_ShouldNotProcess_WhenNoReplayLinkFound()
    {
        // Arrange
        _roomsManager.GetRoomBotConfigurationParameterValue(Arg.Any<string>(), Arg.Any<string>())
            .Returns("true");
        _context.Message.Returns("This is a message without a replay link.");

        // Act
        await _replaysHandler.HandleMessage(_context);

        // Assert
        await _httpService.DidNotReceiveWithAnyArgs().GetAsync<ReplayDto>(default);
    }

    [Test]
    public async Task Test_HandleMessage_ShouldSendHtml_WhenReplayInfoFetchedSuccessfully()
    {
        // Arrange
        var replayUrl = "https://replay.pokemonshowdown.com/gen8ou-123456789";
        _roomsManager.GetRoomBotConfigurationParameterValue(Arg.Any<string>(), Arg.Any<string>())
            .Returns("true");
        _context.Message.Returns(replayUrl);

        var replayData = new ReplayDto
        {
            Format = "gen8ou",
            Rating = 1400,
            Players = ["player1", "player2"],
            UploadTime = 1625140800,
            Views = 200,
            Log = "sample-log-data"
        };

        _httpService.GetAsync<ReplayDto>(replayUrl + ".json").Returns(new HttpResponse<ReplayDto> { Data = replayData });
        _templatesManager.GetTemplate("Replays/ReplayPreview", Arg.Any<ReplayPreviewViewModel>())
            .Returns("sample-html-template");

        // Act
        await _replaysHandler.HandleMessage(_context);

        // Assert
        await _httpService.Received(1).GetAsync<ReplayDto>(replayUrl + ".json");
        _context.Received(1).SendHtml("sample-html-template");
    }

    [Test]
    public async Task Test_HandleMessage_ShouldLogError_WhenExceptionOccurs()
    {
        // Arrange
        var replayUrl = "https://replay.pokemonshowdown.com/gen8ou-123456789";
        _roomsManager.GetRoomBotConfigurationParameterValue(Arg.Any<string>(), Arg.Any<string>())
            .Returns("true");
        _context.Message.Returns(replayUrl);

        _httpService.GetAsync<ReplayDto>(replayUrl + ".json").Throws(new Exception("Request failed"));

        // Act
        await _replaysHandler.HandleMessage(_context);

        // Assert
        _context.DidNotReceive().SendHtml(Arg.Any<string>());
    }
}