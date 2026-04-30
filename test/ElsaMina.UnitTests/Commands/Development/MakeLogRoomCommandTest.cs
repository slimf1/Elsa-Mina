using ElsaMina.Commands.Development;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Logging;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Development;

[TestFixture]
public class MakeLogRoomCommandTest
{
    private IContext _context;
    private IBot _bot;
    private IConfiguration _configuration;
    private MakeLogRoomCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _bot = Substitute.For<IBot>();
        _configuration = Substitute.For<IConfiguration>();
        _configuration.Name.Returns("ElsaMina");
        _command = new MakeLogRoomCommand(_bot, _configuration);

        ShowdownSink.BotSender = null;
        ShowdownSink.RoomId = null;
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
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_RunAsync_ShouldCreateGroupchat_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns("botdev");

        await _command.RunAsync(_context);

        _bot.Received(1).Say("botdev", "/makegroupchat logs");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetSinkRoomId_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns("botdev");

        await _command.RunAsync(_context);

        Assert.That(ShowdownSink.RoomId, Is.EqualTo("groupchat-elsamina-logs"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetSinkBotSender_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns("botdev");

        await _command.RunAsync(_context);

        Assert.That(ShowdownSink.BotSender, Is.Not.Null);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithRoomId_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns("botdev");

        await _command.RunAsync(_context);

        _context.Received(1).Reply("Logging room created: groupchat-elsamina-logs");
    }

    [Test]
    public async Task Test_RunAsync_ShouldJoinRoom_WhenTargetRoomIdProvided()
    {
        _context.Target.Returns("botdev");

        await _command.RunAsync(_context);

        _context.Received(1).Reply("/join botdev");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetSinkRoomId_WhenTargetRoomIdProvided()
    {
        _context.Target.Returns("botdev");

        await _command.RunAsync(_context);

        Assert.That(ShowdownSink.RoomId, Is.EqualTo("botdev"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetSinkBotSender_WhenTargetRoomIdProvided()
    {
        _context.Target.Returns("botdev");

        await _command.RunAsync(_context);

        Assert.That(ShowdownSink.BotSender, Is.Not.Null);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithRoomId_WhenTargetRoomIdProvided()
    {
        _context.Target.Returns("botdev");

        await _command.RunAsync(_context);

        _context.Received(1).Reply("Logging redirected to: botdev");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotCallMakegroupchat_WhenTargetRoomIdProvided()
    {
        _context.Target.Returns("botdev");

        await _command.RunAsync(_context);

        _bot.DidNotReceiveWithAnyArgs().Say(default, default);
    }
}
