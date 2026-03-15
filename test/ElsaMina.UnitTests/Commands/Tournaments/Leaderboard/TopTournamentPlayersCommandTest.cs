using System.Globalization;
using ElsaMina.Commands.Tournaments.Leaderboard;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Tournaments.Leaderboard;

public class TopTournamentPlayersCommandTest
{
    private IContext _context;
    private ITemplatesManager _templatesManager;
    private IBotDbContextFactory _dbContextFactory;
    private IRoomsManager _roomsManager;
    private TopTournamentPlayersCommand _command;
    private DbContextOptions<BotDbContext> _dbOptions;

    private static async Task SeedPlayerAsync(BotDbContext dbContext, string userId, string roomId,
        int winsCount = 0, int runnerUpCount = 0, int thirdPlaceCount = 0,
        int wonGames = 0, int playedGames = 0, int tournamentsEnteredCount = 1,
        string userName = null)
    {
        var user = await dbContext.Users.FindAsync([userId]);
        if (user == null)
        {
            user = new SavedUser { UserId = userId, UserName = userName ?? userId };
            await dbContext.Users.AddAsync(user);
        }

        var roomUser = await dbContext.RoomUsers.FindAsync([userId, roomId]);
        if (roomUser == null)
        {
            roomUser = new RoomUser { Id = userId, RoomId = roomId };
            await dbContext.RoomUsers.AddAsync(roomUser);
        }

        await dbContext.TournamentRecords.AddAsync(new TournamentRecord
        {
            UserId = userId,
            RoomId = roomId,
            WinsCount = winsCount,
            RunnerUpCount = runnerUpCount,
            ThirdPlaceCount = thirdPlaceCount,
            WonGames = wonGames,
            PlayedGames = playedGames,
            TournamentsEnteredCount = tournamentsEnteredCount
        });
    }

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns("arcade");
        _context.Culture.Returns(CultureInfo.InvariantCulture);

        _templatesManager = Substitute.For<ITemplatesManager>();
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>()).Returns("<div/>");

        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbOptions = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(new BotDbContext(_dbOptions)));

        _roomsManager = Substitute.For<IRoomsManager>();
        _roomsManager.GetRoom(Arg.Any<string>()).Returns((IRoom)null);

        _command = new TopTournamentPlayersCommand(_dbContextFactory, _templatesManager, _roomsManager);
    }

    [TearDown]
    public async Task TearDown()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await dbContext.Database.EnsureDeletedAsync();
    }

    [Test]
    public void Test_RequiredRank_ShouldBeRegular()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Regular));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNoData_WhenNoRecordsExist()
    {
        await _command.RunAsync(_context);

        _context.Received(1).ReplyRankAwareLocalizedMessage("top_tournament_players_no_data");
        await _templatesManager.DidNotReceive().GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyError_WhenDatabaseThrows()
    {
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("db error"));

        await _command.RunAsync(_context);

        _context.Received(1).ReplyRankAwareLocalizedMessage("top_tournament_players_error");
        await _templatesManager.DidNotReceive().GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldCallTemplateWithCorrectKeyAndViewModel_WhenRecordsExist()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await SeedPlayerAsync(dbContext, "alice", "arcade", winsCount: 3);
        await SeedPlayerAsync(dbContext, "bob", "arcade", winsCount: 1);
        await dbContext.SaveChangesAsync();

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            "Tournaments/Leaderboard/TopTournamentPlayersTable",
            Arg.Is<TopTournamentPlayersViewModel>(vm =>
                vm.Room == "arcade" &&
                vm.Culture.Name == "" &&
                vm.TopList.Count() == 2));
    }

    [Test]
    public async Task Test_RunAsync_ShouldExcludeRecordsFromOtherRooms()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await SeedPlayerAsync(dbContext, "alice", "arcade", winsCount: 5);
        await SeedPlayerAsync(dbContext, "bob", "otherroom", winsCount: 10);
        await dbContext.SaveChangesAsync();

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<TopTournamentPlayersViewModel>(vm =>
                vm.TopList.Count() == 1 &&
                vm.TopList.First().UserId == "alice"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldOrderByWinsCountDescending()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await SeedPlayerAsync(dbContext, "alice", "arcade", winsCount: 1);
        await SeedPlayerAsync(dbContext, "charlie", "arcade", winsCount: 5);
        await SeedPlayerAsync(dbContext, "bob", "arcade", winsCount: 3);
        await dbContext.SaveChangesAsync();

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<TopTournamentPlayersViewModel>(vm =>
                vm.TopList.ElementAt(0).UserId == "charlie" && vm.TopList.ElementAt(0).Rank == 1 &&
                vm.TopList.ElementAt(1).UserId == "bob" && vm.TopList.ElementAt(1).Rank == 2 &&
                vm.TopList.ElementAt(2).UserId == "alice" && vm.TopList.ElementAt(2).Rank == 3));
    }

    [Test]
    public async Task Test_RunAsync_ShouldOrderByRunnerUpCountDescending_WhenWinsCountIsTied()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await SeedPlayerAsync(dbContext, "alice", "arcade", winsCount: 2, runnerUpCount: 1);
        await SeedPlayerAsync(dbContext, "bob", "arcade", winsCount: 2, runnerUpCount: 3);
        await dbContext.SaveChangesAsync();

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<TopTournamentPlayersViewModel>(vm =>
                vm.TopList.ElementAt(0).UserId == "bob" &&
                vm.TopList.ElementAt(1).UserId == "alice"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldOrderByThirdPlaceCountDescending_WhenWinsAndRunnerUpAreTied()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await SeedPlayerAsync(dbContext, "alice", "arcade", winsCount: 1, runnerUpCount: 1, thirdPlaceCount: 4);
        await SeedPlayerAsync(dbContext, "bob", "arcade", winsCount: 1, runnerUpCount: 1, thirdPlaceCount: 1);
        await dbContext.SaveChangesAsync();

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<TopTournamentPlayersViewModel>(vm =>
                vm.TopList.ElementAt(0).UserId == "alice" &&
                vm.TopList.ElementAt(1).UserId == "bob"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldLimitResultsToTwenty()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        foreach (var index in Enumerable.Range(1, 25))
        {
            await SeedPlayerAsync(dbContext, $"user{index}", "arcade", winsCount: index);
        }
        await dbContext.SaveChangesAsync();

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<TopTournamentPlayersViewModel>(vm => vm.TopList.Count() == 20));
    }

    [Test]
    public async Task Test_RunAsync_ShouldFallbackToUserId_WhenUserNameIsNull()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await SeedPlayerAsync(dbContext, "orphan", "arcade", winsCount: 1, userName: null);
        await dbContext.SaveChangesAsync();

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<TopTournamentPlayersViewModel>(vm =>
                vm.TopList.First().UserName == "orphan"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseUserNameFromRoomUser_WhenPresent()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await SeedPlayerAsync(dbContext, "alice", "arcade", winsCount: 1, userName: "Alice");
        await dbContext.SaveChangesAsync();

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<TopTournamentPlayersViewModel>(vm =>
                vm.TopList.First().UserName == "Alice"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseTargetRoomId_WhenTargetIsProvided()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await SeedPlayerAsync(dbContext, "alice", "otherroom", winsCount: 2);
        await dbContext.SaveChangesAsync();

        _context.Target.Returns("OtherRoom");

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<TopTournamentPlayersViewModel>(vm =>
                vm.TopList.Count() == 1 &&
                vm.TopList.First().UserId == "alice"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseRoomNameFromRoomsManager_WhenRoomExists()
    {
        await using var dbContext = new BotDbContext(_dbOptions);
        await SeedPlayerAsync(dbContext, "alice", "arcade", winsCount: 1);
        await dbContext.SaveChangesAsync();

        var room = Substitute.For<IRoom>();
        room.Name.Returns("Arcade");
        _roomsManager.GetRoom("arcade").Returns(room);

        await _command.RunAsync(_context);

        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<TopTournamentPlayersViewModel>(vm => vm.Room == "Arcade"));
    }
}
