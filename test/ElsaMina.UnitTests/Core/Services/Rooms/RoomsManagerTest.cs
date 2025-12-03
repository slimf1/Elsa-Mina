using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Room = ElsaMina.DataAccess.Models.Room;

namespace ElsaMina.UnitTests.Core.Services.Rooms;

public class RoomsManagerTest
{
    private IConfiguration _configuration;
    private IParametersDefinitionFactory _parametersDefinitionFactory;
    private IRoomUserDataService _roomUserDataService;
    private IDependencyContainerService _container;

    private BotDbContext _db;
    private IBotDbContextFactory _dbFactory;

    private RoomsManager _roomsManager;

    private class TestDbFactory : IBotDbContextFactory
    {
        private readonly DbContextOptions<BotDbContext> _options;
        public TestDbFactory(DbContextOptions<BotDbContext> options) => _options = options;

        public Task<BotDbContext> CreateDbContextAsync(CancellationToken token = default)
            => Task.FromResult(new BotDbContext(_options));
    }

    [SetUp]
    public void SetUp()
    {
        // In-memory EF Core DB
        var opts = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new BotDbContext(opts);
        _dbFactory = new TestDbFactory(opts);

        // Mocks
        _configuration = Substitute.For<IConfiguration>();
        _configuration.DefaultLocaleCode.Returns("en-US");
        _configuration.PlayTimeUpdatesInterval.Returns(TimeSpan.FromDays(1));

        _parametersDefinitionFactory = Substitute.For<IParametersDefinitionFactory>();
        _parametersDefinitionFactory.GetParametersDefinitions()
            .Returns(new Dictionary<Parameter, IParameterDefiniton>()
            {
                [Parameter.Locale] = new ParameterDefinition
                {
                    DefaultValue = "fr-FR",
                    Identifier = "locale",
                    Type = RoomBotConfigurationType.String,
                    NameKey = null,
                    DescriptionKey = null
                }
            });

        _roomUserDataService = Substitute.For<IRoomUserDataService>();

        // Container returns a real RoomParameterStore
        _container = Substitute.For<IDependencyContainerService>();
        _container.Resolve<IRoomParameterStore>().Returns(new EfRoomParameterStore(_dbFactory, _parametersDefinitionFactory));

        // System under test
        _roomsManager = new RoomsManager(
            _configuration,
            _parametersDefinitionFactory,
            _dbFactory,
            _roomUserDataService,
            _container
        );
    }

    [TearDown]
    public void TearDown()
    {
        _roomsManager.Dispose();
        _db.Dispose();
    }

    private async Task InitializeFakeRooms()
    {
        const string roomId1 = "my-room";
        const string roomTitle1 = "My Room";
        string[] linesRoom1 =
        {
            ">" + roomId1,
            "|init|chat",
            "|title|" + roomTitle1,
            "|users|3,&Test,+James@!, Dude"
        };

        const string roomId2 = "franais";
        const string roomTitle2 = "Français";
        string[] linesRoom2 =
        {
            ">" + roomId2,
            "|init|chat",
            "|title|" + roomTitle2,
            "|users|4,&Teclis,!Lionyx,@Earth, Mec"
        };

        await _roomsManager.InitializeRoomAsync(roomId1, linesRoom1);
        await _roomsManager.InitializeRoomAsync(roomId2, linesRoom2);
    }

    // ------------------------------------------------------------
    // TESTS BEGIN
    // ------------------------------------------------------------

    [Test]
    public async Task Test_InitializeRoom_ShouldUseDefaultLocale_WhenRoomParametersDoesntExist()
    {
        // Arrange
        _configuration.DefaultLocaleCode.Returns("zh-CN");

        // Act
        await _roomsManager.InitializeRoomAsync("franais", Array.Empty<string>());

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Culture.Name, Is.EqualTo("zh-CN"));
    }

    [Test]
    public async Task Test_InitializeRoom_ShouldUserLocaleStoredInDb_WhenRoomParametersExist()
    {
        // Arrange
        _db.RoomInfo.Add(new Room
        {
            Id = "franais",
            Title = "Français",
            ParameterValues =
            {
                new RoomBotParameterValue
                {
                    ParameterId = "locale",
                    Value = "fr-FR"
                }
            }
        });
        await _db.SaveChangesAsync();

        // Act
        await _roomsManager.InitializeRoomAsync("franais", Array.Empty<string>());

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
            Assert.That(_roomsManager.GetRoom("my-room"), Is.Not.Null);
            Assert.That(_roomsManager.GetRoom("franais"), Is.Not.Null);
            Assert.That(_roomsManager.GetRoom("my-room").RoomId, Is.EqualTo("my-room"));
            Assert.That(_roomsManager.GetRoom("my-room").Name, Is.EqualTo("My Room"));
            Assert.That(_roomsManager.GetRoom("my-room").Users.Count, Is.EqualTo(3));
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
        Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("polaire"), Is.True);
    }

    [Test]
    public async Task Test_AddUserToRoom_ShouldDoNothingWhenRoomDoesntExist()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.AddUserToRoom("espaol", "&speks");

        // Assert
        Assert.That(_roomsManager.GetRoom("espaol"), Is.Null);
        Assert.That(_roomsManager.GetRoom("franais").Users.Count, Is.EqualTo(4));
    }

    [Test]
    public async Task Test_RemoveUserFromRoom_ShouldRemoveUserFromRoom_WhenRoomAndUserExist()
    {
        // Arrange
        await InitializeFakeRooms();

        // Act
        _roomsManager.RemoveUserFromRoom("franais", "@Earth");

        // Assert
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
            Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("mec"), Is.False);
            Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("dieusupreme"), Is.True);
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
        Assert.That(_roomsManager.GetRoom("my-room").Users["james"].IsIdle, Is.False);
    }

    [Test]
    public async Task Test_ProcessPendingPlayTimeUpdates_ShouldUpdatePlayTime()
    {
        // Arrange
        await _roomsManager.InitializeRoomAsync("myRoom", Array.Empty<string>());
        var room = _roomsManager.GetRoom("myRoom");
        room.PendingPlayTimeUpdates["speks"] = TimeSpan.FromMinutes(90);

        // Act
        await _roomsManager.ProcessPendingPlayTimeUpdates();

        // Assert
        await _roomUserDataService.Received(1)
            .IncrementUserPlayTime("myRoom", "speks", TimeSpan.FromMinutes(90));
        Assert.That(room.PendingPlayTimeUpdates.ContainsKey("speks"), Is.False);
    }

    [Test]
    public async Task Test_ProcessPendingPlayTimeUpdates_ShouldContinue_WhenUpdateThrowsException()
    {
        // Arrange
        await _roomsManager.InitializeRoomAsync("room3", Array.Empty<string>());
        var room = _roomsManager.GetRoom("room3");
        room.PendingPlayTimeUpdates["user3"] = TimeSpan.FromMinutes(15);

        _roomUserDataService
            .When(r => r.IncrementUserPlayTime("room3", "user3", TimeSpan.FromMinutes(15)))
            .Do(_ => throw new InvalidOperationException("Boom"));

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _roomsManager.ProcessPendingPlayTimeUpdates());
        Assert.That(room.PendingPlayTimeUpdates.ContainsKey("user3"), Is.True);
    }

    [Test]
    public async Task Test_ProcessPendingPlayTimeUpdates_ShouldDoNothing_WhenNoRooms()
    {
        // Arrange
        _roomsManager.Clear();

        // Act
        await _roomsManager.ProcessPendingPlayTimeUpdates();

        // Assert
        await _roomUserDataService.DidNotReceiveWithAnyArgs()
            .IncrementUserPlayTime(default!, default!, default);
    }
}
