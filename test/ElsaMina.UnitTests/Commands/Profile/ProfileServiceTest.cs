using ElsaMina.Commands.Profile;
using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Profile;

public class ProfileServiceTest
{
    private IUserDetailsManager _userDetailsManager;
    private ITemplatesManager _templatesManager;
    private IUserDataService _userDataService;
    private IShowdownRanksProvider _showdownRanksProvider;
    private IFormatsManager _formatsManager;
    private IBotDbContextFactory _dbContextFactory;
    private IRoomsManager _roomsManager;
    private BotDbContext _dbContext;
    private ProfileService _sut;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new BotDbContext(options);

        _userDetailsManager = Substitute.For<IUserDetailsManager>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _userDataService = Substitute.For<IUserDataService>();
        _showdownRanksProvider = Substitute.For<IShowdownRanksProvider>();
        _formatsManager = Substitute.For<IFormatsManager>();
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _roomsManager = Substitute.For<IRoomsManager>();

        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(_dbContext));
        _userDetailsManager.GetUserDetailsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((UserDetailsDto)null);
        _userDataService.GetRegisterDateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(DateTimeOffset.MinValue);
        _showdownRanksProvider.GetRankingDataAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((IEnumerable<RankingDataDto>)null);
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>())
            .Returns("rendered");

        _sut = new ProfileService(
            _userDetailsManager,
            _templatesManager,
            _userDataService,
            _showdownRanksProvider,
            _formatsManager,
            _dbContextFactory,
            _roomsManager);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetPlayTime_FromStoredUserData()
    {
        // Arrange
        var expectedPlayTime = TimeSpan.FromHours(3.5);
        _dbContext.RoomUsers.Add(new RoomUser
        {
            Id = "alice",
            RoomId = "room1",
            PlayTime = expectedPlayTime,
            User = new SavedUser { UserId = "alice", UserName = "Alice" }
        });
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.PlayTime == expectedPlayTime));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetPlayTimeToZero_WhenNoUserDataExists()
    {
        // Act
        await _sut.GetProfileHtmlAsync("unknown", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.PlayTime == TimeSpan.Zero));
    }

    #region GameRecords

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetGameRecordsHasAnyRecordToFalse_WhenNoGameDataExists()
    {
        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => !vm.GameRecords.HasAnyRecord));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetFloodIt_WhenFloodItScoreExists()
    {
        // Arrange
        _dbContext.FloodItScores.Add(new FloodItScore { UserId = "alice", Level = 7, BestMoves = 14, TotalStars = 3 });
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm =>
                vm.GameRecords.FloodIt != null &&
                vm.GameRecords.FloodIt.Level == 7 &&
                vm.GameRecords.FloodIt.TotalStars == 3));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetFloodItToNull_WhenNoFloodItScoreExists()
    {
        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.GameRecords.FloodIt == null));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldNotReturnOtherUsersFloodItScore()
    {
        // Arrange
        _dbContext.FloodItScores.Add(new FloodItScore { UserId = "bob", Level = 10, BestMoves = 5, TotalStars = 3 });
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.GameRecords.FloodIt == null));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetLightsOut_WhenLightsOutScoreExists()
    {
        // Arrange
        _dbContext.LightsOutScores.Add(new LightsOutScore { UserId = "alice", Level = 9, BestMoves = 8, TotalStars = 2 });
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm =>
                vm.GameRecords.LightsOut != null &&
                vm.GameRecords.LightsOut.Level == 9 &&
                vm.GameRecords.LightsOut.TotalStars == 2));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetLightsOutToNull_WhenNoLightsOutScoreExists()
    {
        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.GameRecords.LightsOut == null));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetVoltorbFlip_WhenVoltorbFlipLevelExists()
    {
        // Arrange
        _dbContext.VoltorbFlipLevels.Add(new VoltorbFlipLevel
        {
            UserId = "alice",
            Level = 3,
            MaxLevel = 6,
            Coins = 1500
        });
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm =>
                vm.GameRecords.VoltorbFlip != null &&
                vm.GameRecords.VoltorbFlip.MaxLevel == 6 &&
                vm.GameRecords.VoltorbFlip.Coins == 1500));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetVoltorbFlipToNull_WhenNoVoltorbFlipLevelExists()
    {
        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.GameRecords.VoltorbFlip == null));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetTwentyFortyEight_WhenScoreExists()
    {
        // Arrange
        _dbContext.TwentyFortyEightScores.Add(new TwentyFortyEightScore
        {
            UserId = "alice",
            BestScore = 8192,
            Wins = 4
        });
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm =>
                vm.GameRecords.TwentyFortyEight != null &&
                vm.GameRecords.TwentyFortyEight.BestScore == 8192 &&
                vm.GameRecords.TwentyFortyEight.Wins == 4));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetTwentyFortyEightToNull_WhenNoScoreExists()
    {
        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.GameRecords.TwentyFortyEight == null));
    }

    [Test]
    public async Task Test_GetProfileHtmlAsync_ShouldSetHasAnyRecordToTrue_WhenAtLeastOneGameRecordExists()
    {
        // Arrange
        _dbContext.TwentyFortyEightScores.Add(new TwentyFortyEightScore
        {
            UserId = "alice",
            BestScore = 1024,
            Wins = 1
        });
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.GetProfileHtmlAsync("alice", "room1");

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.GameRecords.HasAnyRecord));
    }

    #endregion

    #region GetAvatar

    [Test]
    public void Test_GetAvatar_ShouldReturnDefaultAvatar_WhenNoStoredOrShowdownAvatar()
    {
        var avatar = ProfileService.GetAvatar(null, null);

        Assert.That(avatar, Is.EqualTo("https://play.pokemonshowdown.com/sprites/trainers/unknown.png"));
    }

    [Test]
    public void Test_GetAvatar_ShouldReturnCustomStoredAvatar_WhenPresent()
    {
        var stored = new RoomUser { Avatar = "https://custom/avatar.png" };

        var avatar = ProfileService.GetAvatar(stored, null);

        Assert.That(avatar, Is.EqualTo("https://custom/avatar.png"));
    }

    [Test]
    public void Test_GetAvatar_ShouldReturnCustomUrl_WhenAvatarStartsWithHash()
    {
        var details = new UserDetailsDto { Avatar = "#123" };

        var avatar = ProfileService.GetAvatar(null, details);

        Assert.That(avatar.Contains("trainers-custom/123.png"), Is.True);
    }

    #endregion
}
