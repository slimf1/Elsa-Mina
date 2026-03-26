using System.Globalization;
using ElsaMina.Commands.Development;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Development;

[TestFixture]
public class UptimeCommandTest
{
    private IContext _context;
    private IBot _bot;
    private UptimeCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _bot = Substitute.For<IBot>();
        _command = new UptimeCommand(_bot);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeVoiced()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFormattedUptime()
    {
        var uptime = new TimeSpan(1, 2, 30, 0);
        _bot.UpTime.Returns(uptime);
        _context.Culture.Returns(CultureInfo.InvariantCulture);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("uptime", uptime.ToString("g", CultureInfo.InvariantCulture));
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseContextCultureForFormatting()
    {
        var uptime = new TimeSpan(0, 0, 45, 0);
        var culture = new CultureInfo("fr-FR");
        _bot.UpTime.Returns(uptime);
        _context.Culture.Returns(culture);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("uptime", uptime.ToString("g", culture));
    }
}
