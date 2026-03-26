using ElsaMina.Commands.Arcade.Levels;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Arcade.Levels;

[TestFixture]
public class GetArcadeLevelCommandTest
{
    private DbContextOptions<BotDbContext> _dbOptions;
    private IBotDbContextFactory _dbContextFactory;
    private IContext _context;
    private GetArcadeLevelCommand _command;

    [SetUp]
    public void SetUp()
    {
        _dbOptions = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_ => new BotDbContext(_dbOptions));
        _context = Substitute.For<IContext>();
        _command = new GetArcadeLevelCommand(_dbContextFactory);
    }

    [TearDown]
    public async Task TearDown()
    {
        await using var db = new BotDbContext(_dbOptions);
        await db.Database.EnsureDeletedAsync();
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
    public void Test_HelpMessageKey_ShouldBeCorrect()
    {
        Assert.That(_command.HelpMessageKey, Is.EqualTo("arcade_level_get_help"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelp_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);

        await _command.RunAsync(_context);

        _context.Received(1).Reply(Arg.Any<string>(), rankAware: Arg.Any<bool>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelp_WhenTargetIsWhitespace()
    {
        _context.Target.Returns("   ");

        await _command.RunAsync(_context);

        _context.Received(1).Reply(Arg.Any<string>(), rankAware: Arg.Any<bool>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNotFound_WhenUserHasNoLevel()
    {
        _context.Target.Returns("unknownuser");

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_level_get_not_found", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplySuccess_WhenUserHasLevel()
    {
        await using (var db = new BotDbContext(_dbOptions))
        {
            await db.ArcadeLevels.AddAsync(new ArcadeLevel { Id = "someuser", Level = 15 });
            await db.SaveChangesAsync();
        }

        _context.Target.Returns("someuser");

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_level_get_success", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizeUsername_WhenLookingUpLevel()
    {
        await using (var db = new BotDbContext(_dbOptions))
        {
            await db.ArcadeLevels.AddAsync(new ArcadeLevel { Id = "someuser", Level = 7 });
            await db.SaveChangesAsync();
        }

        _context.Target.Returns("Some User");

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_level_get_success", Arg.Any<object[]>());
    }
}
