using System.Globalization;
using ElsaMina.Commands.Profile;
using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Linq.Expressions;

namespace ElsaMina.UnitTests.Commands.Profile;

public class ProfileCommandTest
{
    private ProfileCommand _command;
    private IBotDbContextFactory _dbContextFactory;
    private IUserDetailsManager _userDetailsManager;
    private ITemplatesManager _templatesManager;
    private IUserDataService _userDataService;
    private IShowdownRanksProvider _showdownRanksProvider;
    private IFormatsManager _formatsManager;
    private IContext _context;
    private BotDbContext _dbContext; 
    private DbSet<RoomUser> _roomUsersDbSet;

    [SetUp]
    public void SetUp()
    {
        // Arrange
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContext = Substitute.For<BotDbContext>();
        
        // Configure the factory to return the mocked context
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(_dbContext));

        // Configure the mocked context to return a substitute for DbSet<RoomUser>
        _roomUsersDbSet = Substitute.For<DbSet<RoomUser>>();
        _dbContext.RoomUsers.Returns(_roomUsersDbSet); 
        _dbContext.Set<RoomUser>().Returns(_roomUsersDbSet); 
        
        _userDetailsManager = Substitute.For<IUserDetailsManager>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _userDataService = Substitute.For<IUserDataService>();
        _showdownRanksProvider = Substitute.For<IShowdownRanksProvider>();
        _formatsManager = Substitute.For<IFormatsManager>();

        _command = new ProfileCommand(
            _userDetailsManager,
            _templatesManager,
            _userDataService,
            _showdownRanksProvider,
            _formatsManager,
            _dbContextFactory
        );

        _context = Substitute.For<IContext>();
        _context.Sender.Returns(Substitute.For<IUser>());
        _context.RoomId.Returns("testroom");
        _context.Sender.UserId.Returns("senderuser"); 
        _context.Room.Returns(Substitute.For<IRoom>());
        _context.Room.Culture.Returns(new CultureInfo("en-US"));
    }

    private void MockRoomUsersQuery(RoomUser result)
    {
        _roomUsersDbSet
            .FirstOrDefaultAsync(Arg.Any<Expression<Func<RoomUser, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(result));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReturnUserProfileTemplate_WhenUserIdIsFound()
    {
        // Arrange
        var targetUserId = "user1";
        var roomUser = new RoomUser 
        { 
            Id = targetUserId, 
            RoomId = _context.RoomId, 
            Avatar = "custom.png", 
            Badges = new List<BadgeHolding>() 
        };
        var userDetails = new UserDetailsDto { Name = "User One", Avatar = "1", Rooms = new Dictionary<string, UserDetailsRoomDto> { { "%testroom", new UserDetailsRoomDto() } } };
        var bestRanking = new RankingDataDto { FormatId = "gen9ou", Elo = 1500 };

        _context.Target.Returns(targetUserId);

        MockRoomUsersQuery(roomUser); 
        _userDetailsManager.GetUserDetailsAsync(targetUserId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(userDetails));
        _userDataService.GetRegisterDateAsync(targetUserId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new DateTimeOffset(2025,2,4,8,30,0,TimeSpan.Zero)));
        _showdownRanksProvider.GetRankingDataAsync(targetUserId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<RankingDataDto>>(new[] { bestRanking }));
        _formatsManager.GetCleanFormat(bestRanking.FormatId).Returns("OU"); 
        _templatesManager.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
            .Returns(Task.FromResult("<html>Profile template</html>"));
        
        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync("Profile/Profile", Arg.Any<object>());
        _context.Received(1).ReplyHtml("<html>Profile template</html>", rankAware: true);
        await _dbContextFactory.Received(1).CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReturnError_WhenUserDetailsNotFound()
    {
        // Arrange
        var targetUserId = "unknownuser";
        _context.Target.Returns(targetUserId);

        MockRoomUsersQuery(null); 
        _userDetailsManager.GetUserDetailsAsync(targetUserId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<UserDetailsDto>(null));
        _userDataService.GetRegisterDateAsync(targetUserId, Arg.Any<CancellationToken>()).ReturnsNull();
        _showdownRanksProvider.GetRankingDataAsync(targetUserId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<RankingDataDto>>(null));
        _templatesManager.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
            .Returns(Task.FromResult("<html>Default template</html>"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync("Profile/Profile", Arg.Is<ProfileViewModel>(vm =>
            vm.Avatar == "https://play.pokemonshowdown.com/sprites/trainers/unknown.png"
            && vm.UserName == targetUserId 
            && vm.UserId == targetUserId
            && vm.UserRoomRank == ' '
            && string.IsNullOrEmpty(vm.Status)
            && vm.BestRanking == null
        ));
        
        _context.Received(1).ReplyHtml("<html>Default template</html>", rankAware: true);
    }

    [Test]
    public void Test_GetAvatar_ShouldReturnCustomAvatar_WhenUserHasCustomAvatar()
    {
        // Arrange
        var userData = new RoomUser { Avatar = "https://custom.avatar/url" };

        // Act
        var avatar = ProfileCommand.GetAvatar(userData, null);

        // Assert
        Assert.That(avatar, Is.EqualTo("https://custom.avatar/url"));
    }

    [Test]
    public void Test_GetAvatar_ShouldReturnDefaultAvatar_WhenNoCustomAvatarIsPresent()
    {
        // Arrange
        var userDetails = new UserDetailsDto { Avatar = "3" };

        // Act
        var avatar = ProfileCommand.GetAvatar(null, userDetails);

        // Assert
        Assert.That(avatar, Is.EqualTo("https://play.pokemonshowdown.com/sprites/trainers/youngster-gen4dp.png"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReturnDefaultProfile_WhenUserIdIsNull()
    {
        // Arrange
        _context.Target.ReturnsNull();
        var senderUserId = _context.Sender.UserId;

        var roomUser = new RoomUser { Id = senderUserId, RoomId = _context.RoomId };
        var userDetails = new UserDetailsDto { Name = "Sender User", Avatar = "1" };
        
        MockRoomUsersQuery(roomUser);
        _userDetailsManager.GetUserDetailsAsync(senderUserId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(userDetails));
        _userDataService.GetRegisterDateAsync(senderUserId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new DateTimeOffset(2025,2,4,8,30,0,TimeSpan.Zero)));
        _showdownRanksProvider.GetRankingDataAsync(senderUserId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<RankingDataDto>>(null));
        _templatesManager.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
            .Returns(Task.FromResult("<html>Sender profile</html>"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync("Profile/Profile", Arg.Any<object>());
        _context.Received(1).ReplyHtml("<html>Sender profile</html>", rankAware: true);
    }
}