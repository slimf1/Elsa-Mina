using ElsaMina.Commands.Arcade.Inscriptions;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Arcade.Inscriptions;

[TestFixture]
public class ArcadeStopCommandTest
{
    private IArcadeInscriptionsManager _inscriptionsManager;
    private IContext _context;
    private IRoom _room;
    private ArcadeStopCommand _command;

    [SetUp]
    public void SetUp()
    {
        _inscriptionsManager = Substitute.For<IArcadeInscriptionsManager>();
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _context.RoomId.Returns("testroom");
        _context.Room.Returns(_room);
        _room.Users.Returns(new Dictionary<string, IUser>());
        _command = new ArcadeStopCommand(_inscriptionsManager);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeCorrect()
    {
        Assert.That(_command.HelpMessageKey, Is.EqualTo("arcade_stop_help"));
    }

    [Test]
    public void Test_RunAsync_ShouldReplyNoActiveInscriptions_WhenNoneActive()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_no_active_inscriptions");
        _inscriptionsManager.DidNotReceive().StopInscriptions(Arg.Any<string>());
    }

    [Test]
    public void Test_RunAsync_ShouldStopInscriptionsAndReplyHtml_WhenParticipantsExist()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);
        var state = new ArcadeRoomState { IsActive = true, Title = "Test Tournament" };
        state.Participants.Add("player1");
        state.Participants.Add("player2");
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        _inscriptionsManager.Received(1).StopInscriptions("testroom");
        _context.Received(1).ReplyHtml(Arg.Any<string>());
    }

    [Test]
    public void Test_RunAsync_ShouldReplyNoParticipants_WhenStateHasNoParticipants()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);
        var state = new ArcadeRoomState { IsActive = true, Title = "Test Tournament" };
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        _inscriptionsManager.Received(1).StopInscriptions("testroom");
        _context.Received(1).ReplyLocalizedMessage("arcade_stop_no_participants");
    }

    [Test]
    public void Test_RunAsync_ShouldIncludeParticipantNamesInHtml_WhenUsersAreInRoom()
    {
        var user = Substitute.For<IUser>();
        user.Name.Returns("Player One");
        _room.Users.Returns(new Dictionary<string, IUser> { ["player1"] = user });

        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);
        var state = new ArcadeRoomState { IsActive = true, Title = "Test" };
        state.Participants.Add("player1");
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });

        _command.RunAsync(_context);

        _context.Received(1).ReplyHtml(Arg.Is<string>(html => html.Contains("Player One")));
    }
}
