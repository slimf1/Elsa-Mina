using ElsaMina.Commands.Misc.Legacy;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Misc.Legacy;

public class WeebifyCommandTest
{
    private IRandomService _randomService;
    private IContext _context;
    private WeebifyCommand _command;

    [SetUp]
    public void SetUp()
    {
        _randomService = Substitute.For<IRandomService>();
        _context = Substitute.For<IContext>();
        _command = new WeebifyCommand(_randomService);
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
    public async Task Test_RunAsync_ShouldAppendSuffixToEachWord()
    {
        // Arrange
        _context.Target.Returns("hello world");
        _randomService.RandomElement(Arg.Is<IEnumerable<string>>(e => e.Contains("-"))).Returns("-");
        _randomService.RandomElement(Arg.Is<IEnumerable<string>>(e => e.Contains("sama"))).Returns("kun");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply(Arg.Is<string>(s =>
            s.Contains("hello-kun") && s.Contains("world-kun")), rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldCallRandomElementTwicePerWord()
    {
        // Arrange
        _context.Target.Returns("a b c");
        _randomService.RandomElement(Arg.Any<IEnumerable<string>>()).Returns("x");

        // Act
        await _command.RunAsync(_context);

        // Assert
        // 3 words × 2 calls (link + suffix) = 6 total
        _randomService.Received(6).RandomElement(Arg.Any<IEnumerable<string>>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithResultPrefix()
    {
        // Arrange
        _context.Target.Returns("test");
        _randomService.RandomElement(Arg.Any<IEnumerable<string>>()).Returns("x");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply(Arg.Is<string>(s => s.StartsWith("Result:")), rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleSingleWord()
    {
        // Arrange
        _context.Target.Returns("naruto");
        _randomService.RandomElement(Arg.Is<IEnumerable<string>>(e => e.Contains("-"))).Returns(" ");
        _randomService.RandomElement(Arg.Is<IEnumerable<string>>(e => e.Contains("sama"))).Returns("senpai");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply(Arg.Is<string>(s => s.Contains("naruto senpai")), rankAware: true);
    }
}
