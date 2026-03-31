using ElsaMina.Commands.FloodIt;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Commands.FloodIt;

public class FloodItColorCommandTest
{
    private IRoomsManager _roomsManager;
    private IFloodItGameManager _gameManager;
    private FloodItColorCommand _command;
    private IContext _context;
    private IRoom _room;
    private IFloodItGame _floodItGame;
    private IUser _sender;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _gameManager = Substitute.For<IFloodItGameManager>();
        _command = new FloodItColorCommand(_roomsManager, _gameManager);
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _floodItGame = Substitute.For<IFloodItGame>();
        _sender = Substitute.For<IUser>();

        _context.Sender.Returns(_sender);
        _sender.UserId.Returns("testplayer");

        _room.Game.Returns(_floodItGame);
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
    public async Task Test_RunAsync_ShouldCallFloodFill_WhenTargetIsValid()
    {
        _context.Target.Returns("test-room, 2");

        await _command.RunAsync(_context);

        await _floodItGame.Received(1).FloodFill(_sender, 2);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenTargetHasTooFewParts()
    {
        _context.Target.Returns("test-room");

        await _command.RunAsync(_context);

        await _floodItGame.DidNotReceive().FloodFill(Arg.Any<IUser>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenColorIndexIsNotAnInteger()
    {
        _context.Target.Returns("test-room, abc");

        await _command.RunAsync(_context);

        await _floodItGame.DidNotReceive().FloodFill(Arg.Any<IUser>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomDoesNotExist()
    {
        _context.Target.Returns("nonexistent-room, 2");
        _roomsManager.GetRoom("nonexistent-room").ReturnsNull();

        await _command.RunAsync(_context);

        await _floodItGame.DidNotReceive().FloodFill(Arg.Any<IUser>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNoFloodItGame()
    {
        _context.Target.Returns("test-room, 2");
        _room.Game.Returns(Substitute.For<IGame>());

        await _command.RunAsync(_context);

        await _floodItGame.DidNotReceive().FloodFill(Arg.Any<IUser>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNullGame()
    {
        _context.Target.Returns("test-room, 2");
        _room.Game.ReturnsNull();

        await _command.RunAsync(_context);

        await _floodItGame.DidNotReceive().FloodFill(Arg.Any<IUser>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseGameFromManager_WhenPrivateModeGameExists()
    {
        var privateGame = Substitute.For<IFloodItGame>();
        privateGame.IsPrivateMode.Returns(true);
        _gameManager.GetGame("test-room", "testplayer").Returns(privateGame);
        _context.Target.Returns("test-room, 3");

        await _command.RunAsync(_context);

        await privateGame.Received(1).FloodFill(_sender, 3);
        await _floodItGame.DidNotReceive().FloodFill(Arg.Any<IUser>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldUpdateContext_WhenPrivateModeGame()
    {
        var privateGame = Substitute.For<IFloodItGame>();
        privateGame.IsPrivateMode.Returns(true);
        _gameManager.GetGame("test-room", "testplayer").Returns(privateGame);
        _context.Target.Returns("test-room, 3");

        await _command.RunAsync(_context);

        Assert.That(privateGame.Context, Is.SameAs(_context));
    }
}
