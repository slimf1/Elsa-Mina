using ElsaMina.Commands.Development.LagTest;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Development.LagTest;

[TestFixture]
public class LagTestCommandTest
{
    private ILagTestManager _lagTestManager;
    private IContext _context;
    private LagTestCommand _command;

    private const string TEST_ROOM_ID = "testroom";

    [SetUp]
    public void SetUp()
    {
        _lagTestManager = Substitute.For<ILagTestManager>();
        _context = Substitute.For<IContext>();

        _context.RoomId.Returns(TEST_ROOM_ID);

        _command = new LagTestCommand(_lagTestManager);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeRegular()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Regular));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendMarkerMessage_BeforeAwaiting()
    {
        // Arrange
        _lagTestManager.StartLagTestAsync(TEST_ROOM_ID, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(TimeSpan.FromMilliseconds(50)));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply(LagTestManager.LAG_TEST_MARKER);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithResult_WhenLagTestSucceeds()
    {
        // Arrange
        var elapsed = TimeSpan.FromMilliseconds(42);
        _lagTestManager.StartLagTestAsync(TEST_ROOM_ID, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(elapsed));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyRankAwareLocalizedMessage("lagtest_result", 42L);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyTimeout_WhenLagTestTimesOut()
    {
        // Arrange
        _lagTestManager.StartLagTestAsync(TEST_ROOM_ID, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(TimeSpan.MinValue));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyRankAwareLocalizedMessage("lagtest_timeout");
        _context.DidNotReceive().ReplyRankAwareLocalizedMessage("lagtest_result", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldStartLagTestWithCorrectRoomId()
    {
        // Arrange
        _lagTestManager.StartLagTestAsync(TEST_ROOM_ID, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(TimeSpan.FromMilliseconds(10)));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _lagTestManager.Received(1).StartLagTestAsync(TEST_ROOM_ID, Arg.Any<CancellationToken>());
    }
}
