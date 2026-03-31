using ElsaMina.Commands.FloodIt;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Commands.FloodIt;

public class EndFloodItCommandTest
{
    // Concrete IUser to avoid NSubstitute auto-substitute sharing between Owner and Sender
    private record TestUser(string UserId) : IUser
    {
        public string Name => UserId;
        public bool IsIdle => false;
        public Rank Rank => Rank.Regular;
    }

    private IRoomsManager _roomsManager;
    private IFloodItGameManager _gameManager;
    private EndFloodItCommand _command;
    private IContext _context;
    private IRoom _room;
    private IFloodItGame _floodItGame;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _gameManager = Substitute.For<IFloodItGameManager>();
        _command = new EndFloodItCommand(_roomsManager, _gameManager);
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _floodItGame = Substitute.For<IFloodItGame>();
    }

    [Test]
    public void Test_RequiredRank_ShouldReturnVoiced()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    #region Room mode

    [Test]
    public async Task Test_RunAsync_ShouldCancelGame_WhenFloodItGameExists()
    {
        _context.Room.Returns(_room);
        _room.Game.Returns(_floodItGame);

        await _command.RunAsync(_context);

        await _floodItGame.Received(1).CancelAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyCancelled_WhenFloodItGameExists()
    {
        _context.Room.Returns(_room);
        _room.Game.Returns(_floodItGame);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("fi_game_cancelled");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNoGame_WhenRoomHasNoGame()
    {
        _context.Room.Returns(_room);
        _room.Game.ReturnsNull();

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("fi_game_no_game");
        await _floodItGame.DidNotReceive().CancelAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNoGame_WhenOtherGameTypeExists()
    {
        _context.Room.Returns(_room);
        _room.Game.Returns(Substitute.For<IGame>());

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("fi_game_no_game");
        await _floodItGame.DidNotReceive().CancelAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNotOwner_WhenSenderIsNotOwner()
    {
        var owner = new TestUser("owner-id");
        var sender = new TestUser("sender-id");
        _floodItGame.Owner = owner;
        _context.Sender.Returns(sender);
        _context.Room.Returns(_room);
        _room.Game.Returns(_floodItGame);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("fi_game_not_owner");
        await _floodItGame.DidNotReceive().CancelAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldCancelGame_WhenSenderIsNotOwnerButHasDriverRank()
    {
        var owner = new TestUser("owner-id");
        var sender = new TestUser("sender-id");
        _floodItGame.Owner = owner;
        _context.Sender.Returns(sender);
        _context.HasRankOrHigher(Rank.Driver).Returns(true);
        _context.Room.Returns(_room);
        _room.Game.Returns(_floodItGame);

        await _command.RunAsync(_context);

        await _floodItGame.Received(1).CancelAsync();
        _context.Received(1).ReplyLocalizedMessage("fi_game_cancelled");
    }

    [Test]
    public async Task Test_RunAsync_ShouldCancelGame_WhenSenderIsOwner()
    {
        var owner = new TestUser("owner-id");
        _floodItGame.Owner = owner;
        _context.Sender.Returns(owner);
        _context.Room.Returns(_room);
        _room.Game.Returns(_floodItGame);

        await _command.RunAsync(_context);

        await _floodItGame.Received(1).CancelAsync();
    }

    #endregion

    #region Private message mode

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenPmAndTargetIsEmpty()
    {
        _context.IsPrivateMessage.Returns(true);
        _context.Target.Returns(string.Empty);

        await _command.RunAsync(_context);

        await _floodItGame.DidNotReceive().CancelAsync();
        _context.DidNotReceive().ReplyLocalizedMessage(Arg.Any<string>(), Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNoGame_WhenPmAndNoGameExistsForRoom()
    {
        _context.IsPrivateMessage.Returns(true);
        _context.Target.Returns("test-room");
        _context.Sender.Returns(new TestUser("testplayer"));
        _gameManager.GetGame("test-room", "testplayer").ReturnsNull();

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("fi_game_no_game");
        await _floodItGame.DidNotReceive().CancelAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldCancelGame_WhenPmAndGameExistsForRoom()
    {
        _context.IsPrivateMessage.Returns(true);
        _context.Target.Returns("test-room");
        var sender = new TestUser("testplayer");
        _context.Sender.Returns(sender);
        _gameManager.GetGame("test-room", "testplayer").Returns(_floodItGame);
        _roomsManager.GetRoom("test-room").Returns(_room);
        _floodItGame.Context = _context;

        await _command.RunAsync(_context);

        await _floodItGame.Received(1).CancelAsync();
        _context.Received(1).ReplyLocalizedMessage("fi_game_cancelled");
    }

    #endregion
}
