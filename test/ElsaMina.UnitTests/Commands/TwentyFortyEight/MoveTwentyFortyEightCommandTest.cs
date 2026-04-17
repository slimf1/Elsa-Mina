using ElsaMina.Commands.TwentyFortyEight;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Games;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Commands.TwentyFortyEight;

public class MoveTwentyFortyEightCommandTest
{
    private IRoomsManager _roomsManager;
    private ITwentyFortyEightGameManager _gameManager;
    private MoveTwentyFortyEightCommand _command;
    private IContext _context;
    private IRoom _room;
    private ITwentyFortyEightGame _game;
    private IUser _sender;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _gameManager = Substitute.For<ITwentyFortyEightGameManager>();
        _command = new MoveTwentyFortyEightCommand(_roomsManager, _gameManager);
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _game = Substitute.For<ITwentyFortyEightGame>();
        _sender = Substitute.For<IUser>();

        _context.Sender.Returns(_sender);
        _sender.UserId.Returns("testplayer");

        _room.Game.Returns(_game);
        _roomsManager.GetRoom("test-room").Returns(_room);
        _gameManager.GetGame(Arg.Any<string>(), Arg.Any<string>()).ReturnsNull();
    }

    [Test]
    public void Test_IsPrivateMessageOnly_ShouldBeTrue()
    {
        Assert.That(_command.IsPrivateMessageOnly, Is.True);
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_RunAsync_ShouldCallMakeMove_WhenTargetIsValid()
    {
        _context.Target.Returns("test-room, left");

        await _command.RunAsync(_context);

        await _game.Received(1).MakeMove(_sender, "left");
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenTargetHasTooFewParts()
    {
        _context.Target.Returns("test-room");

        await _command.RunAsync(_context);

        await _game.DidNotReceive().MakeMove(Arg.Any<IUser>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomDoesNotExist()
    {
        _context.Target.Returns("nonexistent-room, left");
        _roomsManager.GetRoom("nonexistent-room").ReturnsNull();

        await _command.RunAsync(_context);

        await _game.DidNotReceive().MakeMove(Arg.Any<IUser>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNoTwentyFortyEightGame()
    {
        _context.Target.Returns("test-room, left");
        _room.Game.Returns(Substitute.For<IGame>());

        await _command.RunAsync(_context);

        await _game.DidNotReceive().MakeMove(Arg.Any<IUser>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNullGame()
    {
        _context.Target.Returns("test-room, left");
        _room.Game.ReturnsNull();

        await _command.RunAsync(_context);

        await _game.DidNotReceive().MakeMove(Arg.Any<IUser>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseGameFromManager_WhenPrivateModeGameExists()
    {
        var privateGame = Substitute.For<ITwentyFortyEightGame>();
        privateGame.IsPrivateMode.Returns(true);
        _gameManager.GetGame("test-room", "testplayer").Returns(privateGame);
        _context.Target.Returns("test-room, up");

        await _command.RunAsync(_context);

        await privateGame.Received(1).MakeMove(_sender, "up");
        await _game.DidNotReceive().MakeMove(Arg.Any<IUser>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldUpdateContext_WhenPrivateModeGame()
    {
        var privateGame = Substitute.For<ITwentyFortyEightGame>();
        privateGame.IsPrivateMode.Returns(true);
        _gameManager.GetGame("test-room", "testplayer").Returns(privateGame);
        _context.Target.Returns("test-room, down");

        await _command.RunAsync(_context);

        Assert.That(privateGame.Context, Is.SameAs(_context));
    }
}
