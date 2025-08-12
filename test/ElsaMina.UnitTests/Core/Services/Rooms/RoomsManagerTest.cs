using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Core.Services.Rooms;

public class RoomsManagerTest
{
    private IConfiguration _configuration;
    private IRoomInfoRepository _roomInfoRepository;
    private IParametersFactory _parametersFactory;
    private IRoomBotParameterValueRepository _roomBotParameterValueRepository;
    private IUserPlayTimeRepository _userPlayTimeRepository;
    private IClockService _clockService;

    private RoomsManager _roomsManager;

    [SetUp]
    public void SetUp()
    {
        _configuration = Substitute.For<IConfiguration>();
        _roomInfoRepository = Substitute.For<IRoomInfoRepository>();
        _parametersFactory = Substitute.For<IParametersFactory>();
        _roomBotParameterValueRepository = Substitute.For<IRoomBotParameterValueRepository>();
        _userPlayTimeRepository = Substitute.For<IUserPlayTimeRepository>();
        _clockService = Substitute.For<IClockService>();

        _configuration.PlayTimeUpdatesInterval.Returns(TimeSpan.FromDays(1));
        _roomsManager = new RoomsManager(_configuration, _parametersFactory,
            _roomInfoRepository, _roomBotParameterValueRepository, _userPlayTimeRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _roomsManager.Dispose();
    }

    private async Task InitializeFakeRooms()
    {
        const string roomId1 = "my-room";
        const string roomTitle1 = "My Room";
        string[] linesRoom1 =
        [
            ">" + roomId1,
            "|init|chat",
            "|title|"+ roomTitle1,
            "|users|3,&Test,+James@!, Dude"
        ];

        const string roomId2 = "franais";
        const string roomTitle2 = "Français";
        string[] linesRoom2 =
        [
            ">" + roomId2,
            "|init|chat",
            "|title|"+ roomTitle2,
            "|users|4,&Teclis,!Lionyx,@Earth, Mec"
        ];

        await _roomsManager.InitializeRoomAsync(roomId1, linesRoom1);
        await _roomsManager.InitializeRoomAsync(roomId2, linesRoom2);
    }

    [Test]
    public async Task Test_InitializeRoom_ShouldUseDefaultLocale_WhenRoomParametersDoesntExist()
    {
        // Arrange
        _configuration.DefaultLocaleCode.Returns("zh-CN");
        _roomInfoRepository.GetByIdAsync("franais").ReturnsNull();

        // Act
        await _roomsManager.InitializeRoomAsync("franais", []);

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Culture.Name, Is.EqualTo("zh-CN"));
    }

    [Test]
    public async Task Test_InitializeRoom_ShouldUserLocaleStoredInDb_WhenRoomParametersExist()
    {
        // Arrange
        _roomInfoRepository.GetByIdAsync("franais").Returns(new RoomInfo
        {
            Id = "franais",
            ParameterValues = new List<RoomBotParameterValue>
            {
                new()
                {
                    ParameterId = ParametersConstants.LOCALE,
                    Value = "fr-FR"
                }
            }
        });

        // Act
        await _roomsManager.InitializeRoomAsync("franais", []);

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Culture.Name, Is.EqualTo("fr-FR"));
    }

    [Test]
    public async Task Test_InitializeRoom_ShouldAddRoom()
    {
        // Arrange & Act
        await InitializeFakeRooms();

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(_roomsManager.HasRoom("my-room"), Is.True);
            Assert.That(_roomsManager.HasRoom("franais"), Is.True);
            Assert.That(_roomsManager.HasRoom("doesn't exist"), Is.False);
            Assert.That(_roomsManager.GetRoom("my-room"), Is.Not.Null);
            Assert.That(_roomsManager.GetRoom("franais"), Is.Not.Null);
            Assert.That(_roomsManager.GetRoom("doesn't exist"), Is.Null);
            Assert.That(_roomsManager.GetRoom("my-room").RoomId, Is.EqualTo("my-room"));
            Assert.That(_roomsManager.GetRoom("my-room").Name, Is.EqualTo("My Room"));
            Assert.That(_roomsManager.GetRoom("my-room").Users, Has.Count.EqualTo(3));
            Assert.That(_roomsManager.GetRoom("my-room").Users.ContainsKey("test"), Is.True);
            Assert.That(_roomsManager.GetRoom("my-room").Users["test"].IsIdle, Is.False);
            Assert.That(_roomsManager.GetRoom("my-room").Users.ContainsKey("james"), Is.True);
            Assert.That(_roomsManager.GetRoom("my-room").Users["james"].IsIdle, Is.True);
            Assert.That(_roomsManager.GetRoom("my-room").Users.ContainsKey("dude"), Is.True);
            Assert.That(_roomsManager.GetRoom("my-room").Users["dude"].IsIdle, Is.False);

            Assert.That(_roomsManager.GetRoom("franais").RoomId, Is.EqualTo("franais"));
            Assert.That(_roomsManager.GetRoom("franais").Name, Is.EqualTo("Français"));
            Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(4));
            Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("teclis"), Is.True);
            Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("lionyx"), Is.True);
            Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("earth"), Is.True);
            Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("mec"), Is.True);
        });
    }

    [Test]
    public async Task Test_AddUserToRoom_ShouldAddUserToRoom_WhenRoomExists()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.AddUserToRoom("franais", "%Polaire");

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(5));
        Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("polaire"), Is.True);
    }

    [Test]
    public async Task Test_AddUserToRoom_ShouldDoNothingWhenRoomDoesntExist()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.AddUserToRoom("espaol", "&speks");
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(_roomsManager.GetRoom("espaol"), Is.Null);
            Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(4));
        });
    }

    [Test]
    public async Task Test_RemoveUserFromRoom_ShouldRemoveUserFromRoom_WhenRoomAndUserExist()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.RemoveUserFromRoom("franais", "@Earth");

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(3));
        Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("earth"), Is.False);
    }

    [Test]
    public async Task Test_RemoveUserFromRoom_ShouldDoNothing_WhenRoomExistsButUserDoesnt()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.RemoveUserFromRoom("franais", "+Corentin");

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(4));
        Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("corentin"), Is.False);
    }

    [Test]
    public async Task Test_RemoveUserFromRoom_ShouldDoNothing_WhenRoomDoesntExist()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.RemoveUserFromRoom("espaol", "@Earth");

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(4));
        Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("earth"), Is.True);
    }

    [Test]
    public async Task Test_RenameUserInRoom_ShouldRenameUser()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.RenameUserInRoom("franais", "mec", "&DieuSupreme");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(4));
            Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("mec"), Is.False);
            Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("dieusupreme"), Is.True);
            Assert.That(_roomsManager.GetRoom("franais").Users["dieusupreme"].Name, Is.EqualTo("DieuSupreme"));
            Assert.That(_roomsManager.GetRoom("franais").Users["dieusupreme"].Rank, Is.EqualTo(Rank.Leader));
        });
    }

    [Test]
    public async Task Test_RenameUserInRoom_ShouldRenameUser_WhenUserIsAfk()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.RenameUserInRoom("franais", "teclis", "&Teclis@!");

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(4));
        Assert.That(_roomsManager.GetRoom("franais").Users["teclis"].IsIdle, Is.True);
    }

    [Test]
    public async Task Test_RenameUserInRoom_ShouldRenameUser_WhenUserIsNotAfkAnymore()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.RenameUserInRoom("my-room", "james", "+James");

        // Assert
        Assert.That(_roomsManager.GetRoom("my-room").Users, Has.Count.EqualTo(3));
        Assert.That(_roomsManager.GetRoom("my-room").Users["james"].IsIdle, Is.False);
    }
    
    [Test]
    public async Task Test_ProcessPendingPlayTimeUpdates_ShouldUpdatePlayTime_WhenPlayTimeDoesExist()
    {
        // Arrange
        await _roomsManager.InitializeRoomAsync("myRoom", []);
        var room = _roomsManager.GetRoom("myRoom");
        room.PendingPlayTimeUpdates["speks"] = TimeSpan.FromMinutes(90);
        room.RoomId.Returns("myRoom");
        _userPlayTimeRepository.GetByIdAsync(Tuple.Create("speks", "myRoom")).Returns(new UserPlayTime
        {
            PlayTime = new TimeSpan(10, 30, 0),
            UserId = "speks",
            RoomId = "myRoom"
        });
        _clockService.CurrentUtcDateTime.Returns(new DateTime(2022, 10, 1, 22, 00, 0, DateTimeKind.Utc));

        // Act
        await _roomsManager.ProcessPendingPlayTimeUpdates();
        
        // Assert
        await _userPlayTimeRepository.Received(1).UpdateAsync(Arg.Is<UserPlayTime>(playTime => Math.Abs(playTime.PlayTime.TotalHours - 12) < 1e-3
            && playTime.UserId == "speks"
            && playTime.RoomId == "myRoom"));
    }
    
    [Test]
public async Task Test_ProcessPendingPlayTimeUpdates_ShouldAddPlayTime_WhenNoExistingRecord()
{
    // Arrange
    await _roomsManager.InitializeRoomAsync("room1", []);

    var room = _roomsManager.GetRoom("room1");
    room.PendingPlayTimeUpdates["user1"] = TimeSpan.FromMinutes(90);

    _userPlayTimeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>())
        .ReturnsNull();

    // Act
    await _roomsManager.ProcessPendingPlayTimeUpdates();

    // Assert
    await _userPlayTimeRepository.Received(1).AddAsync(Arg.Is<UserPlayTime>(pt =>
        pt.UserId == "user1" &&
        pt.RoomId == "room1" &&
        Math.Abs(pt.PlayTime.TotalMinutes - 90) < 1e-3), Arg.Any<CancellationToken>());
    Assert.That(room.PendingPlayTimeUpdates.ContainsKey("user1"), Is.False);
}

[Test]
public async Task Test_ProcessPendingPlayTimeUpdates_ShouldUpdatePlayTime_WhenExistingRecordFound()
{
    // Arrange
    await _roomsManager.InitializeRoomAsync("room2", []);

    var room = _roomsManager.GetRoom("room2");
    room.PendingPlayTimeUpdates["user2"] = TimeSpan.FromMinutes(30);

    var existing = new UserPlayTime
    {
        UserId = "user2",
        RoomId = "room2",
        PlayTime = TimeSpan.FromMinutes(10)
    };
    _userPlayTimeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>())
        .Returns(existing);

    // Act
    await _roomsManager.ProcessPendingPlayTimeUpdates();

    // Assert
    await _userPlayTimeRepository.Received(1).UpdateAsync(Arg.Is<UserPlayTime>(pt =>
        Math.Abs(pt.PlayTime.TotalMinutes - 40) < 1e-3));
    Assert.That(room.PendingPlayTimeUpdates.ContainsKey("user2"), Is.False);
}

[Test]
public async Task Test_ProcessPendingPlayTimeUpdates_ShouldContinue_WhenUpdateThrowsException()
{
    // Arrange
    await _roomsManager.InitializeRoomAsync("room3", []);

    var room = _roomsManager.GetRoom("room3");
    room.PendingPlayTimeUpdates["user3"] = TimeSpan.FromMinutes(15);

    _userPlayTimeRepository
        .When(r => r.GetByIdAsync(Arg.Any<Tuple<string, string>>()))
        .Do(_ => throw new InvalidOperationException("Boom"));

    // Act & Assert
    Assert.DoesNotThrowAsync(() => _roomsManager.ProcessPendingPlayTimeUpdates());
    Assert.That(room.PendingPlayTimeUpdates.ContainsKey("user3"), Is.True);
}

[Test]
public async Task Test_ProcessPendingPlayTimeUpdates_ShouldDoNothing_WhenNoRooms()
{
    // Arrange
    // No initialization — _rooms stays empty

    // Act
    await _roomsManager.ProcessPendingPlayTimeUpdates();

    // Assert
    await _userPlayTimeRepository.DidNotReceiveWithAnyArgs().AddAsync(default!);
    await _userPlayTimeRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
}
}