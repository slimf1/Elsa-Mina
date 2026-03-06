using ElsaMina.Commands.Misc.Legacy;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Misc.Legacy;

public class ElectionCommandTest
{
    private IRandomService _randomService;
    private IContext _context;
    private ElectionCommand _command;

    [SetUp]
    public void SetUp()
    {
        _randomService = Substitute.For<IRandomService>();
        _context = Substitute.For<IContext>();
        _command = new ElectionCommand(_randomService);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeVoiced()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Regular));
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotReply_WhenTargetIsEmpty()
    {
        // Arrange
        _context.Target.Returns("");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _randomService.DidNotReceive().NextDouble();
        _context.DidNotReceive().Reply(Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotReply_WhenTargetIsWhitespaceOnly()
    {
        // Arrange
        _context.Target.Returns("  ,  ,  ");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _randomService.DidNotReceive().NextDouble();
        _context.DidNotReceive().Reply(Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldCallNextDoubleOncePerCandidate()
    {
        // Arrange
        _context.Target.Returns("Alice, Bob, Charlie");
        _randomService.NextDouble().Returns(0.5, 0.3, 0.2);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _randomService.Received(3).NextDouble();
    }

    [Test]
    public async Task Test_RunAsync_ShouldIncludeAllCandidatesInReply()
    {
        // Arrange
        _context.Target.Returns("Alice, Bob");
        _randomService.NextDouble().Returns(0.6, 0.4);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply(Arg.Is<string>(s =>
            s.Contains("Alice") && s.Contains("Bob")), rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithResultsPrefix()
    {
        // Arrange
        _context.Target.Returns("Alice");
        _randomService.NextDouble().Returns(1.0);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply(Arg.Is<string>(s => s.StartsWith("Results:")), rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizePercentagesToSumTo100()
    {
        // Arrange
        _context.Target.Returns("Alice, Bob");
        // Alice gets 0.75, Bob gets 0.25 → total 1.0 → Alice 75%, Bob 25%
        _randomService.NextDouble().Returns(0.75, 0.25);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply(Arg.Is<string>(s =>
            s.Contains("75.00%") && s.Contains("25.00%")), rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldOrderCandidatesByScoreDescending()
    {
        // Arrange
        _context.Target.Returns("Alice, Bob");
        // Alice scores lower than Bob
        _randomService.NextDouble().Returns(0.2, 0.8);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply(Arg.Is<string>(s =>
            s.IndexOf("Bob", StringComparison.Ordinal) < s.IndexOf("Alice", StringComparison.Ordinal)), rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimWhitespaceFromCandidateNames()
    {
        // Arrange
        _context.Target.Returns("  Alice  ,  Bob  ");
        _randomService.NextDouble().Returns(0.5, 0.5);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply(Arg.Is<string>(s =>
            s.Contains("Alice") && s.Contains("Bob") &&
            !s.Contains("  Alice") && !s.Contains("  Bob")), rankAware: true);
    }
}