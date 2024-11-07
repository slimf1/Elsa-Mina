using ElsaMina.Commands.Profile;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.Profile;

public class ProfileCommandTest
{
    private ProfileCommand _command;
    private IRoomSpecificUserDataRepository _userDataRepository;
    private IUserDetailsManager _userDetailsManager;
    private ITemplatesManager _templatesManager;
    private IRoomsManager _roomsManager;
    private IUserDataService _userDataService;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _userDataRepository = Substitute.For<IRoomSpecificUserDataRepository>();
        _userDetailsManager = Substitute.For<IUserDetailsManager>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _userDataService = Substitute.For<IUserDataService>();

        _command = new ProfileCommand(
            _userDataRepository,
            _userDetailsManager,
            _templatesManager,
            _roomsManager,
            _userDataService
        );

        _context = Substitute.For<IContext>();
        _context.Sender.Returns(Substitute.For<IUser>());
        _context.RoomId.Returns("testRoom");
    }

    [Test]
    public async Task Test_Run_ShouldReturnUserProfileTemplate_WhenUserIdIsFound()
    {
        // Arrange
        _context.Target.Returns("user1");
        var userDetails = new UserDetailsDto { Name = "User One", Avatar = "1" };
        _userDetailsManager.GetUserDetails("user1").Returns(Task.FromResult(userDetails));
        _userDataRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>())
            .Returns(Task.FromResult(new RoomSpecificUserData { Id = "user1", RoomId = "testRoom" }));
        _templatesManager.GetTemplate("Profile/Profile", Arg.Any<object>())
            .Returns(Task.FromResult("<html>Profile template</html>"));

        // Act
        await _command.Run(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplate("Profile/Profile", Arg.Any<object>());
        _context.Received(1).SendHtml(Arg.Any<string>(), "<html>Profile template</html>", true);
    }

    [Test]
    public async Task Test_Run_ShouldReturnError_WhenUserDetailsNotFound()
    {
        // Arrange
        _context.Target.Returns("unknownUser");
        _userDetailsManager.GetUserDetails("unknownUser").Returns(Task.FromResult<UserDetailsDto>(null));

        // Act
        await _command.Run(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplate("Profile/Profile", Arg.Is<ProfileViewModel>(vm =>
            vm.Avatar == "https://play.pokemonshowdown.com/sprites/trainers/unknown.png"
            && vm.UserName == "unknownuser"
            && vm.UserId == "unknownuser"
            && vm.UserRoomRank == ' '
            && string.IsNullOrEmpty(vm.Status)
        ));
    }

    [Test]
    public void Test_GetAvatar_ShouldReturnCustomAvatar_WhenUserHasCustomAvatar()
    {
        // Arrange
        var userData = new RoomSpecificUserData { Avatar = "https://custom.avatar/url" };

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
    public async Task Test_Run_ShouldReturnDefaultProfile_WhenUserIdIsNull()
    {
        // Arrange
        _context.Target.ReturnsNull();

        // Act
        await _command.Run(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplate("Profile/Profile", Arg.Any<object>());
        _context.Received(1).SendHtml(Arg.Any<string>(), Arg.Any<string>(), true);
    }
}