using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Contexts;

public class RoomContextTest
{
    private const string TEST_ROOM_ID = "testRoom";
    private const string TEST_USER_ID = "testUser";

    private IContextProvider _contextProvider;
    private IBot _bot;
    private IUser _sender;
    private IRoom _room;

    private RoomContext _roomContext;

    private void CreateRoomContext(string message, string target, string command, long timestamp)
    {
        _contextProvider = Substitute.For<IContextProvider>();
        _bot = Substitute.For<IBot>();
        _sender = Substitute.For<IUser>();
        _room = Substitute.For<IRoom>();
        _room.RoomId.Returns(TEST_ROOM_ID);
        _sender.UserId.Returns(TEST_USER_ID);

        _roomContext = new RoomContext(
            _contextProvider,
            _bot,
            message,
            target,
            _sender,
            command,
            _room,
            timestamp);
    }

    [Test]
    [TestCase(Rank.Regular, ExpectedResult = false)]
    [TestCase(Rank.Voiced, ExpectedResult = false)]
    [TestCase(Rank.Driver, ExpectedResult = false)]
    [TestCase(Rank.Mod, ExpectedResult = true)]
    [TestCase(Rank.Bot, ExpectedResult = true)]
    [TestCase(Rank.RoomOwner, ExpectedResult = true)]
    [TestCase(Rank.Leader, ExpectedResult = true)]
    [TestCase(Rank.Admin, ExpectedResult = true)]
    public bool Test_HasSufficientRank_ShouldReturnTrue_WhenSenderRankIsSufficient(Rank userRank)
    {
        // Arrange
        CreateRoomContext("", "", "test-command", 1);
        _sender.Rank.Returns(userRank);

        // Act & Assert
        return _roomContext.HasRankOrHigher(Rank.Mod);
    }

    [Test]
    public void Test_HasSufficientRank_ShouldReturnTrueInEveryCase_WhenSenderIsWhitelisted(
        [Values] Rank requiredRank,
        [Values] Rank userRank)
    {
        // Arrange
        CreateRoomContext("", "", "test-command", 1);
        _sender.UserId.Returns("wl-dude");
        _sender.Rank.Returns(userRank);
        _contextProvider.IsUserWhitelisted("wl-dude").Returns(true);

        // Act
        var value = _roomContext.HasRankOrHigher(requiredRank);

        // Assert
        Assert.That(value, Is.True);
    }

    [Test]
    public void Test_Reply_ShouldSendMessageToSender_WhenRankAwareAndInsufficientRank()
    {
        // Arrange
        CreateRoomContext("", "", null, 1);
        _sender.Rank.Returns(Rank.Regular);

        // Act
        _roomContext.Reply("Test message", rankAware: true);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, $"/pm {TEST_USER_ID}, Test message");
    }

    [Test]
    public void Test_Reply_ShouldSendMessageToRoom_WhenNotRankAware()
    {
        // Act
        CreateRoomContext("", "", null, 1);
        _roomContext.Reply("Test message", rankAware: false);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, "Test message");
    }

    [Test]
    public void Test_ReplyHtml_ShouldSendPmInfobox_WhenRankAwareAndInsufficientRank()
    {
        // Arrange
        CreateRoomContext("", "", null, 1);
        _sender.Rank.Returns(Rank.Regular);

        // Act
        _roomContext.ReplyHtml("HTML content", rankAware: true);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, $"/pminfobox {TEST_USER_ID}, HTML content");
    }

    [Test]
    public void Test_ReplyHtml_ShouldSendHtmlToRoom_WhenNotRankAware()
    {
        // Act
        CreateRoomContext("", "", null, 1);
        _roomContext.ReplyHtml("HTML content", rankAware: false);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, "/addhtmlbox HTML content");
    }

    [Test]
    public void Test_ReplyUpdatableHtml_ShouldSendChangeCommand_WhenIsChangingIsTrue()
    {
        // Act
        CreateRoomContext("", "", null, 1);
        _roomContext.ReplyUpdatableHtml("htmlId", "HTML content", isChanging: true);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, "/changeuhtml htmlId, HTML content");
    }

    [Test]
    public void Test_ReplyUpdatableHtml_ShouldSendAddCommand_WhenIsChangingIsFalse()
    {
        // Act
        CreateRoomContext("", "", null, 1);
        _roomContext.ReplyUpdatableHtml("htmlId", "HTML content", isChanging: false);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, "/adduhtml htmlId, HTML content");
    }
}