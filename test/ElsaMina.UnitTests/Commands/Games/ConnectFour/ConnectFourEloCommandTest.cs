using ElsaMina.Commands.Games.ConnectFour;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Games.ConnectFour;

public class ConnectFourEloCommandTest
{
    private DbContextOptions<BotDbContext> _dbOptions;
    private IBotDbContextFactory _dbContextFactory;
    private IContext _context;
    private IUser _sender;
    private ConnectFourEloCommand _command;

    [SetUp]
    public void SetUp()
    {
        _dbOptions = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(callInfo => new BotDbContext(_dbOptions));

        _sender = Substitute.For<IUser>();
        _sender.UserId.Returns("alice");

        _context = Substitute.For<IContext>();
        _context.Sender.Returns(_sender);
        _context.Target.Returns(string.Empty);

        _command = new ConnectFourEloCommand(_dbContextFactory);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeRegular()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Regular));
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNotFound_WhenRatingDoesNotExist()
    {
        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("c4_elo_not_found", "alice");
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseSenderId_WhenTargetIsEmpty()
    {
        await using (var setupContext = new BotDbContext(_dbOptions))
        {
            setupContext.ConnectFourRatings.Add(new ConnectFourRating
            {
                UserId = "alice", Rating = 1100, Wins = 5, Losses = 3, Draws = 1
            });
            await setupContext.SaveChangesAsync();
        }

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("c4_elo_info", "alice", 1100, 5, 3, 1);
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseTargetId_WhenTargetIsProvided()
    {
        _context.Target.Returns("bob");

        await using (var setupContext = new BotDbContext(_dbOptions))
        {
            setupContext.ConnectFourRatings.Add(new ConnectFourRating
            {
                UserId = "bob", Rating = 950, Wins = 2, Losses = 6, Draws = 0
            });
            await setupContext.SaveChangesAsync();
        }

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("c4_elo_info", "bob", 950, 2, 6, 0);
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizeTargetId_WhenTargetHasSpacesAndUppercase()
    {
        _context.Target.Returns("  Bob Smith  ");

        await using (var setupContext = new BotDbContext(_dbOptions))
        {
            setupContext.ConnectFourRatings.Add(new ConnectFourRating
            {
                UserId = "bobsmith", Rating = 1000, Wins = 0, Losses = 0, Draws = 0
            });
            await setupContext.SaveChangesAsync();
        }

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("c4_elo_info", "bobsmith", 1000, 0, 0, 0);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNotFound_WhenTargetHasNoRating()
    {
        _context.Target.Returns("unknown");

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("c4_elo_not_found", "unknown");
    }
}
