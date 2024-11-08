using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using NSubstitute;

namespace ElsaMina.Test.Core.Contexts;

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
    [TestCase(' ', ExpectedResult = false)]
    [TestCase('+', ExpectedResult = false)]
    [TestCase('%', ExpectedResult = false)]
    [TestCase('@', ExpectedResult = true)]
    [TestCase('*', ExpectedResult = true)]
    [TestCase('#', ExpectedResult = true)]
    [TestCase('~', ExpectedResult = true)]
    [TestCase('X', ExpectedResult = false)]
    public bool Test_HasSufficientRank_ShouldReturnTrue_WhenSenderRankIsSufficient(char userRank)
    {
        // Arrange
        CreateRoomContext("", "", "test-command", 1);
        _sender.Rank.Returns(userRank);

        // Act & Assert
        return _roomContext.HasSufficientRank('@');
    }

    [Test]
    public void Test_HasSufficientRank_ShouldReturnTrueInEveryCase_WhenSenderIsWhitelisted(
        [Values(' ', '+', '%', '@', '*', '#', '~')]
        char requiredRank,
        [Values(' ', '+', '%', '@', '*', '#', '~')]
        char userRank)
    {
        // Arrange
        CreateRoomContext("", "", "test-command", 1);
        _sender.UserId.Returns("wl-dude");
        _sender.Rank.Returns(userRank);
        _contextProvider.CurrentWhitelist.Returns(["wl-dude"]);

        // Act
        var value = _roomContext.HasSufficientRank(requiredRank);

        // Assert
        Assert.That(value, Is.True);
    }

    [Test]
    public void Test_Reply_ShouldSendMessageToSender_WhenRankAwareAndInsufficientRank()
    {
        // Arrange
        CreateRoomContext("", "", null, 1);
        _sender.Rank.Returns(' ');

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
    public void Test_SendHtml_ShouldSendPmInfobox_WhenRankAwareAndInsufficientRank()
    {
        // Arrange
        CreateRoomContext("", "", null, 1);
        _sender.Rank.Returns(' ');

        // Act
        _roomContext.SendHtml("HTML content", rankAware: true);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, $"/pminfobox {TEST_USER_ID}, HTML content");
    }

    [Test]
    public void Test_SendHtml_ShouldSendHtmlToRoom_WhenNotRankAware()
    {
        // Act
        CreateRoomContext("", "", null, 1);
        _roomContext.SendHtml("HTML content", rankAware: false);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, "/addhtmlbox HTML content");
    }

    [Test]
    public void Test_SendUpdatableHtml_ShouldSendChangeCommand_WhenIsChangingIsTrue()
    {
        // Act
        CreateRoomContext("", "", null, 1);
        _roomContext.SendUpdatableHtml("htmlId", "HTML content", isChanging: true);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, "/changeuhtml htmlId, HTML content");
    }

    [Test]
    public void Test_SendUpdatableHtml_ShouldSendAddCommand_WhenIsChangingIsFalse()
    {
        // Act
        CreateRoomContext("", "", null, 1);
        _roomContext.SendUpdatableHtml("htmlId", "HTML content", isChanging: false);

        // Assert
        _bot.Received(1).Say(TEST_ROOM_ID, "/adduhtml htmlId, HTML content");
    }
}