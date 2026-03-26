using ElsaMina.Commands.Arcade.Inscriptions;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Arcade.Inscriptions;

[TestFixture]
public class ArcadeAddCommandTest
{
    private IArcadeInscriptionsManager _inscriptionsManager;
    private IContext _context;
    private IRoom _room;
    private IUser _sender;
    private ArcadeAddCommand _command;

    [SetUp]
    public void SetUp()
    {
        _inscriptionsManager = Substitute.For<IArcadeInscriptionsManager>();
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _sender = Substitute.For<IUser>();

        _sender.UserId.Returns("driveruser");
        _sender.Name.Returns("DriverUser");
        _context.RoomId.Returns("testroom");
        _context.Room.Returns(_room);
        _context.Sender.Returns(_sender);
        _room.Users.Returns(new Dictionary<string, IUser>());

        _command = new ArcadeAddCommand(_inscriptionsManager);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeCorrect()
    {
        Assert.That(_command.HelpMessageKey, Is.EqualTo("arcade_add_help"));
    }

    [Test]
    public void Test_RunAsync_ShouldReplyHelp_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);

        _command.RunAsync(_context);

        _context.Received(1).Reply(Arg.Any<string>(), rankAware: Arg.Any<bool>());
    }

    [Test]
    public void Test_RunAsync_ShouldReplyNoData_WhenNoActiveInscriptions()
    {
        _context.Target.Returns("someuser");
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy).Returns(false);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_list_no_data");
    }

    [Test]
    public void Test_RunAsync_ShouldReplyAlreadyRegistered_WhenParticipantExists()
    {
        _context.Target.Returns("Some User");
        var state = new ArcadeRoomState { IsActive = true, Title = "Test" };
        state.Participants.Add("someuser");
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_add_already_registered", "Some User");
    }

    [Test]
    public void Test_RunAsync_ShouldReplyBanned_WhenUserIsBanned()
    {
        _context.Target.Returns("banneduser");
        var state = new ArcadeRoomState { IsActive = true, Title = "Test" };
        state.BannedUsers.Add("banneduser");
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_add_banned", "banneduser");
    }

    [Test]
    public void Test_RunAsync_ShouldAddParticipantAndReplySuccess_WhenUserIsEligible()
    {
        _context.Target.Returns("newuser");
        var state = new ArcadeRoomState { IsActive = true, Title = "Test" };
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        Assert.That(state.Participants, Contains.Item("newuser"));
        _context.Received(1).ReplyLocalizedMessage("arcade_add_success", "newuser");
    }

    [Test]
    public void Test_RunAsync_ShouldNormalizeUserId_WhenAddingParticipant()
    {
        _context.Target.Returns("New User");
        var state = new ArcadeRoomState { IsActive = true, Title = "Test" };
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        Assert.That(state.Participants, Contains.Item("newuser"));
    }
}
