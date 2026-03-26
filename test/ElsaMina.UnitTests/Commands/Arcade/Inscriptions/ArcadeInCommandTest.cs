using ElsaMina.Commands.Arcade.Inscriptions;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Arcade.Inscriptions;

[TestFixture]
public class ArcadeInCommandTest
{
    private IArcadeInscriptionsManager _inscriptionsManager;
    private IRoomsManager _roomsManager;
    private IContext _context;
    private IUser _sender;
    private ArcadeInCommand _command;

    [SetUp]
    public void SetUp()
    {
        _inscriptionsManager = Substitute.For<IArcadeInscriptionsManager>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _context = Substitute.For<IContext>();
        _sender = Substitute.For<IUser>();

        _sender.UserId.Returns("testuser");
        _sender.Name.Returns("TestUser");
        _context.Sender.Returns(_sender);
        _context.RoomId.Returns("testroom");
        _context.IsPrivateMessage.Returns(false);

        _command = new ArcadeInCommand(_inscriptionsManager, _roomsManager);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeRegular()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Regular));
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public void Test_RunAsync_ShouldReplySpecifyRoom_WhenPrivateMessageAndNoTarget()
    {
        _context.IsPrivateMessage.Returns(true);
        _context.Target.Returns(string.Empty);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_in_pm_specify_room");
    }

    [Test]
    public void Test_RunAsync_ShouldReplyRoomNotFound_WhenPrivateMessageAndRoomDoesNotExist()
    {
        _context.IsPrivateMessage.Returns(true);
        _context.Target.Returns("unknownroom");
        _roomsManager.HasRoom("unknownroom").Returns(false);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_pm_room_not_found");
    }

    [Test]
    public void Test_RunAsync_ShouldReplyNoActiveInscriptions_WhenNoneActive()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_no_active_inscriptions");
    }

    [Test]
    public void Test_RunAsync_ShouldReplyTimerExpired_WhenTimerHasExpired()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);
        var state = new ArcadeRoomState { IsActive = true, IsTimerExpired = true };
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_inscriptions_timer_expired");
    }

    [Test]
    public void Test_RunAsync_ShouldReplyBanned_WhenUserIsBanned()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);
        var state = new ArcadeRoomState { IsActive = true };
        state.BannedUsers.Add("testuser");
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_in_banned");
    }

    [Test]
    public void Test_RunAsync_ShouldReplyAlreadyRegistered_WhenUserIsAlreadyParticipant()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);
        var state = new ArcadeRoomState { IsActive = true };
        state.Participants.Add("testuser");
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_in_already_registered");
    }

    [Test]
    public void Test_RunAsync_ShouldAddUserAndReplySuccess_WhenEligible()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);
        var state = new ArcadeRoomState { IsActive = true };
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        Assert.That(state.Participants, Contains.Item("testuser"));
        _context.Received(1).ReplyLocalizedMessage("arcade_in_success", Arg.Any<object[]>());
    }

    [Test]
    public void Test_RunAsync_ShouldSendModnoteViaMP_WhenCommandIsFromPrivateMessage()
    {
        _context.IsPrivateMessage.Returns(true);
        _context.Target.Returns("testroom");
        _roomsManager.HasRoom("testroom").Returns(true);
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);
        var state = new ArcadeRoomState { IsActive = true };
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        _context.Received(1).SendMessageIn("testroom", Arg.Is<string>(msg => msg.Contains("MP")));
    }
}
