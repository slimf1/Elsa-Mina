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
