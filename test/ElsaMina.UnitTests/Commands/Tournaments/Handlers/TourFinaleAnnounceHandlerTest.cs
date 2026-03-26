using ElsaMina.Commands.Tournaments.Handlers;
using ElsaMina.Core;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Tournaments.Handlers;

[TestFixture]
public class TourFinaleAnnounceHandlerTest
{
    private const string INPROGRESS_JSON =
        """{"bracketData":{"rootNode":{"state":"inprogress","room":"battle-gen9ou-123"}}}""";

    private IBot _bot;
    private IResourcesService _resourcesService;
    private IRoomsManager _roomsManager;
    private TourFinaleAnnounceHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _bot = Substitute.For<IBot>();
        _resourcesService = Substitute.For<IResourcesService>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _handler = new TourFinaleAnnounceHandler(_bot, _resourcesService, _roomsManager);
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldDoNothing_WhenPartsAreTooShort()
    {
        var parts = new[] { "", "tournament", "update" };

        await _handler.HandleReceivedMessageAsync(parts, "testroom");

        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldDoNothing_WhenMessageTypeIsNotTournament()
    {
        var parts = new[] { "", "chat", "update", INPROGRESS_JSON };

        await _handler.HandleReceivedMessageAsync(parts, "testroom");

        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldDoNothing_WhenSubTypeIsNotUpdate()
    {
        var parts = new[] { "", "tournament", "end", INPROGRESS_JSON };

        await _handler.HandleReceivedMessageAsync(parts, "testroom");

        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldDoNothing_WhenStateIsNotInProgress()
    {
        const string json = """{"bracketData":{"rootNode":{"state":"finished","room":"battle-gen9ou-123"}}}""";
        var parts = new[] { "", "tournament", "update", json };

        await _handler.HandleReceivedMessageAsync(parts, "testroom");

        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldDoNothing_WhenBattleRoomIsNull()
    {
        const string json = """{"bracketData":{"rootNode":{"state":"inprogress"}}}""";
        var parts = new[] { "", "tournament", "update", json };

        await _handler.HandleReceivedMessageAsync(parts, "testroom");

        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldDoNothing_WhenRoomIsNotFound()
    {
        var parts = new[] { "", "tournament", "update", INPROGRESS_JSON };
        _roomsManager.GetRoom("testroom").Returns((IRoom)null);

        await _handler.HandleReceivedMessageAsync(parts, "testroom");

        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldSayWallMessage_WhenAllConditionsMet()
    {
        var room = Substitute.For<IRoom>();
        room.Culture.Returns(System.Globalization.CultureInfo.InvariantCulture);
        var parts = new[] { "", "tournament", "update", INPROGRESS_JSON };
        _roomsManager.GetRoom("testroom").Returns(room);
        _resourcesService.GetString("tour_finale_announce_message", room.Culture)
            .Returns("Finale: {0}");

        await _handler.HandleReceivedMessageAsync(parts, "testroom");

        _bot.Received(1).Say("testroom", "/wall Finale: battle-gen9ou-123");
    }
}
