using ElsaMina.Commands.Development;
using ElsaMina.Core.Contexts;
using ElsaMina.Core;
using ElsaMina.Logging;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Development;

[TestFixture]
public class DisableLogRoomCommandTest
{
    private IContext _context;
    private DisableLogRoomCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _command = new DisableLogRoomCommand();

        ShowdownSink.BotSender = Substitute.For<IBot>().Say;
        ShowdownSink.RoomId = "some-room";
    }

    [TearDown]
    public void TearDown()
    {
        ShowdownSink.BotSender = null;
        ShowdownSink.RoomId = null;
    }

    [Test]
    public void Test_IsWhitelistOnly_ShouldBeTrue()
    {
        Assert.That(_command.IsWhitelistOnly, Is.True);
    }

    [Test]
    public async Task Test_RunAsync_ShouldClearBotSender()
    {
        await _command.RunAsync(_context);

        Assert.That(ShowdownSink.BotSender, Is.Null);
    }

    [Test]
    public async Task Test_RunAsync_ShouldClearRoomId()
    {
        await _command.RunAsync(_context);

        Assert.That(ShowdownSink.RoomId, Is.Null);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyConfirmation()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("Showdown logging sink disabled.");
    }
}
