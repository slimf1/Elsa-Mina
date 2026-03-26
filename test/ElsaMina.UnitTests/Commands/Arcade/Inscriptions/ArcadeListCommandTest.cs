using ElsaMina.Commands.Arcade.Inscriptions;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Arcade.Inscriptions;

[TestFixture]
public class ArcadeListCommandTest
{
    private IArcadeInscriptionsManager _inscriptionsManager;
    private IContext _context;
    private IRoom _room;
    private ArcadeListCommand _command;

    [SetUp]
    public void SetUp()
    {
        _inscriptionsManager = Substitute.For<IArcadeInscriptionsManager>();
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _context.RoomId.Returns("testroom");
        _context.Room.Returns(_room);
        _room.Users.Returns(new Dictionary<string, IUser>());
        _command = new ArcadeListCommand(_inscriptionsManager);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeVoiced()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeCorrect()
    {
        Assert.That(_command.HelpMessageKey, Is.EqualTo("arcade_list_help"));
    }

    [Test]
    public void Test_RunAsync_ShouldReplyNoData_WhenNoInscriptionsExist()
    {
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy).Returns(false);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_list_no_data");
    }

    [Test]
    public void Test_RunAsync_ShouldReplyNoParticipants_WhenStateExistsButEmpty()
    {
        var state = new ArcadeRoomState { IsActive = true, Title = "Test" };
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });
        _context.GetString(Arg.Any<string>()).Returns("active");

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_list_no_participants", Arg.Any<object[]>());
    }

    [Test]
    public void Test_RunAsync_ShouldReplyResult_WhenParticipantsExist()
    {
        var state = new ArcadeRoomState { IsActive = true, Title = "Test" };
        state.Participants.Add("player1");
        state.Participants.Add("player2");
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });
        _context.GetString(Arg.Any<string>(), Arg.Any<object[]>()).Returns(string.Empty);
        _context.GetString(Arg.Any<string>()).Returns("active");

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_list_result", Arg.Any<object[]>());
    }

    [Test]
    public void Test_RunAsync_ShouldUseUserName_WhenParticipantIsInRoom()
    {
        var user = Substitute.For<IUser>();
        user.Name.Returns("Player One");
        _room.Users.Returns(new Dictionary<string, IUser> { ["player1"] = user });

        var state = new ArcadeRoomState { IsActive = true, Title = "Test" };
        state.Participants.Add("player1");
        ArcadeRoomState dummy = null;
        _inscriptionsManager.TryGetState("testroom", out dummy)
            .Returns(callInfo => { callInfo[1] = state; return true; });
        _context.GetString(Arg.Any<string>(), Arg.Any<object[]>()).Returns(string.Empty);
        _context.GetString(Arg.Any<string>()).Returns("active");

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_list_result",
            Arg.Is<object[]>(args => args.Contains("Player One")));
    }
}
