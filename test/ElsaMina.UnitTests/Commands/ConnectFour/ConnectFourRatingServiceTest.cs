using ElsaMina.Commands.ConnectFour;
using ElsaMina.Core.Services.Elo;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.ConnectFour;

public class ConnectFourRatingServiceTest
{
    private DbContextOptions<BotDbContext> _dbOptions;
    private IBotDbContextFactory _dbContextFactory;
    private IEloService _eloService;
    private ConnectFourRatingService _sut;
    private IUser _mockWinner;
    private IUser _mockLoser;

    [SetUp]
    public void SetUp()
    {
        _dbOptions = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(callInfo => new BotDbContext(_dbOptions));

        _eloService = Substitute.For<IEloService>();
        _eloService.DefaultRating.Returns(1000);
        _eloService.CalculateWinRatings(Arg.Any<int>(), Arg.Any<int>()).Returns((1016, 984));
        _eloService.CalculateDrawRatings(Arg.Any<int>(), Arg.Any<int>()).Returns((1000, 1000));

        _mockWinner = Substitute.For<IUser>();
        _mockWinner.UserId.Returns("winner");
        _mockLoser = Substitute.For<IUser>();
        _mockLoser.UserId.Returns("loser");

        _sut = new ConnectFourRatingService(_dbContextFactory, _eloService);
    }

    #region UpdateRatingsOnWinAsync

    [Test]
    public async Task Test_UpdateRatingsOnWinAsync_ShouldCreateRatings_WhenUsersHaveNoExistingRating()
    {
        await _sut.UpdateRatingsOnWinAsync(_mockWinner, _mockLoser);

        await using var dbContext = new BotDbContext(_dbOptions);
        var winnerRating = await dbContext.ConnectFourRatings.FindAsync("winner");
        var loserRating = await dbContext.ConnectFourRatings.FindAsync("loser");

        Assert.Multiple(() =>
        {
            Assert.That(winnerRating, Is.Not.Null);
            Assert.That(loserRating, Is.Not.Null);
        });
    }

    [Test]
    public async Task Test_UpdateRatingsOnWinAsync_ShouldUseDefaultRating_WhenCreatingNewRatings()
    {
        _eloService.CalculateWinRatings(1000, 1000).Returns((1016, 984));

        await _sut.UpdateRatingsOnWinAsync(_mockWinner, _mockLoser);

        _eloService.Received(1).CalculateWinRatings(1000, 1000);
    }

    [Test]
    public async Task Test_UpdateRatingsOnWinAsync_ShouldUpdateRatings_WhenUsersHaveExistingRatings()
    {
        await using (var setupContext = new BotDbContext(_dbOptions))
        {
            setupContext.ConnectFourRatings.Add(new ConnectFourRating { UserId = "winner", Rating = 1100 });
            setupContext.ConnectFourRatings.Add(new ConnectFourRating { UserId = "loser", Rating = 900 });
            await setupContext.SaveChangesAsync();
        }

        _eloService.CalculateWinRatings(1100, 900).Returns((1108, 892));

        await _sut.UpdateRatingsOnWinAsync(_mockWinner, _mockLoser);

        await using var dbContext = new BotDbContext(_dbOptions);
        var winnerRating = await dbContext.ConnectFourRatings.FindAsync("winner");
        var loserRating = await dbContext.ConnectFourRatings.FindAsync("loser");

        Assert.Multiple(() =>
        {
            Assert.That(winnerRating.Rating, Is.EqualTo(1108));
            Assert.That(loserRating.Rating, Is.EqualTo(892));
        });
    }

    [Test]
    public async Task Test_UpdateRatingsOnWinAsync_ShouldIncrementWinsAndLosses()
    {
        await using (var setupContext = new BotDbContext(_dbOptions))
        {
            setupContext.ConnectFourRatings.Add(new ConnectFourRating { UserId = "winner", Rating = 1000, Wins = 2, Losses = 1 });
            setupContext.ConnectFourRatings.Add(new ConnectFourRating { UserId = "loser", Rating = 1000, Wins = 1, Losses = 2 });
            await setupContext.SaveChangesAsync();
        }

        await _sut.UpdateRatingsOnWinAsync(_mockWinner, _mockLoser);

        await using var dbContext = new BotDbContext(_dbOptions);
        var winnerRating = await dbContext.ConnectFourRatings.FindAsync("winner");
        var loserRating = await dbContext.ConnectFourRatings.FindAsync("loser");

        Assert.Multiple(() =>
        {
            Assert.That(winnerRating.Wins, Is.EqualTo(3));
            Assert.That(winnerRating.Losses, Is.EqualTo(1));
            Assert.That(loserRating.Losses, Is.EqualTo(3));
            Assert.That(loserRating.Wins, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task Test_UpdateRatingsOnWinAsync_ShouldReturnCorrectChanges()
    {
        _eloService.CalculateWinRatings(1000, 1000).Returns((1016, 984));

        var (winnerChange, loserChange) = await _sut.UpdateRatingsOnWinAsync(_mockWinner, _mockLoser);

        Assert.Multiple(() =>
        {
            Assert.That(winnerChange.OldRating, Is.EqualTo(1000));
            Assert.That(winnerChange.NewRating, Is.EqualTo(1016));
            Assert.That(winnerChange.Delta, Is.EqualTo(16));
            Assert.That(loserChange.OldRating, Is.EqualTo(1000));
            Assert.That(loserChange.NewRating, Is.EqualTo(984));
            Assert.That(loserChange.Delta, Is.EqualTo(-16));
        });
    }

    #endregion

    #region UpdateRatingsOnDrawAsync

    [Test]
    public async Task Test_UpdateRatingsOnDrawAsync_ShouldCreateRatings_WhenUsersHaveNoExistingRating()
    {
        await _sut.UpdateRatingsOnDrawAsync(_mockWinner, _mockLoser);

        await using var dbContext = new BotDbContext(_dbOptions);
        var rating1 = await dbContext.ConnectFourRatings.FindAsync("winner");
        var rating2 = await dbContext.ConnectFourRatings.FindAsync("loser");

        Assert.Multiple(() =>
        {
            Assert.That(rating1, Is.Not.Null);
            Assert.That(rating2, Is.Not.Null);
        });
    }

    [Test]
    public async Task Test_UpdateRatingsOnDrawAsync_ShouldIncrementDraws()
    {
        await using (var setupContext = new BotDbContext(_dbOptions))
        {
            setupContext.ConnectFourRatings.Add(new ConnectFourRating { UserId = "winner", Rating = 1000, Draws = 1 });
            setupContext.ConnectFourRatings.Add(new ConnectFourRating { UserId = "loser", Rating = 1000, Draws = 2 });
            await setupContext.SaveChangesAsync();
        }

        await _sut.UpdateRatingsOnDrawAsync(_mockWinner, _mockLoser);

        await using var dbContext = new BotDbContext(_dbOptions);
        var rating1 = await dbContext.ConnectFourRatings.FindAsync("winner");
        var rating2 = await dbContext.ConnectFourRatings.FindAsync("loser");

        Assert.Multiple(() =>
        {
            Assert.That(rating1.Draws, Is.EqualTo(2));
            Assert.That(rating2.Draws, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task Test_UpdateRatingsOnDrawAsync_ShouldReturnCorrectChanges()
    {
        _eloService.CalculateDrawRatings(1000, 1000).Returns((1000, 1000));

        var (change1, change2) = await _sut.UpdateRatingsOnDrawAsync(_mockWinner, _mockLoser);

        Assert.Multiple(() =>
        {
            Assert.That(change1.OldRating, Is.EqualTo(1000));
            Assert.That(change1.NewRating, Is.EqualTo(1000));
            Assert.That(change1.Delta, Is.EqualTo(0));
            Assert.That(change2.OldRating, Is.EqualTo(1000));
            Assert.That(change2.NewRating, Is.EqualTo(1000));
            Assert.That(change2.Delta, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Test_UpdateRatingsOnDrawAsync_ShouldNotIncrementWinsOrLosses()
    {
        await using (var setupContext = new BotDbContext(_dbOptions))
        {
            setupContext.ConnectFourRatings.Add(new ConnectFourRating { UserId = "winner", Rating = 1000, Wins = 3, Losses = 2 });
            setupContext.ConnectFourRatings.Add(new ConnectFourRating { UserId = "loser", Rating = 1000, Wins = 1, Losses = 4 });
            await setupContext.SaveChangesAsync();
        }

        await _sut.UpdateRatingsOnDrawAsync(_mockWinner, _mockLoser);

        await using var dbContext = new BotDbContext(_dbOptions);
        var rating1 = await dbContext.ConnectFourRatings.FindAsync("winner");
        var rating2 = await dbContext.ConnectFourRatings.FindAsync("loser");

        Assert.Multiple(() =>
        {
            Assert.That(rating1.Wins, Is.EqualTo(3));
            Assert.That(rating1.Losses, Is.EqualTo(2));
            Assert.That(rating2.Wins, Is.EqualTo(1));
            Assert.That(rating2.Losses, Is.EqualTo(4));
        });
    }

    #endregion
}
