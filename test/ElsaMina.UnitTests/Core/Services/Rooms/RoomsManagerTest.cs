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
    private RoomsManager _sut;
    private IConfiguration _configuration;
    private IParametersDefinitionFactory _parametersDefinitionFactory;
    private IBotDbContextFactory _dbContextFactory;
    private IRoomUserDataService _roomUserDataService;
    private IDependencyContainerService _dependencyContainerService;
    private DbContextOptions<BotDbContext> _dbContextOptions;
    private IRoomParameterStore _roomParameterStore;

    // Helper method to create unique in-memory options for each test run
    private DbContextOptions<BotDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    // Helper method to mock the factory to return a new context connected to the shared options
    private IBotDbContextFactory CreateFactory(DbContextOptions<BotDbContext> options)
    {
        var factory = Substitute.For<IBotDbContextFactory>();
        factory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(new BotDbContext(options)));
        return factory;
    }

    // Helper method to seed the database
    private async Task SeedDatabaseAsync(IEnumerable<Room> rooms)
    {
        // Arrange
        await using var context = new BotDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();
        context.RoomInfo.AddRange(rooms);
        await context.SaveChangesAsync();
    }

    [SetUp]
    public void Setup()
    {
        // Arrange
        _dbContextOptions = CreateOptions();

        // Mocks
        _configuration = Substitute.For<IConfiguration>();
        _configuration.DefaultLocaleCode.Returns("en-US");
        _configuration.PlayTimeUpdatesInterval.Returns(TimeSpan.FromDays(10)); // Timer interval

        _parametersDefinitionFactory = Substitute.For<IParametersDefinitionFactory>();
        _parametersDefinitionFactory.GetParametersDefinitions().Returns(
            new Dictionary<Parameter, IParameterDefiniton>
            {
                { Parameter.Locale, Substitute.For<IParameterDefiniton>() }
            });

        _dbContextFactory = CreateFactory(_dbContextOptions);
        _roomUserDataService = Substitute.For<IRoomUserDataService>();
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _roomParameterStore = Substitute.For<IRoomParameterStore>();
        _dependencyContainerService.Resolve<IRoomParameterStore>().Returns(_roomParameterStore);
        _roomParameterStore.GetValueAsync(Parameter.Locale, Arg.Any<CancellationToken>()).Returns("fr-FR");

        _dependencyContainerService.Resolve<IRoomParameterStore>().Returns(_roomParameterStore);

        _sut = new RoomsManager(_configuration, _parametersDefinitionFactory, _dbContextFactory, _roomUserDataService,
            _dependencyContainerService);

        // Clear any room state from previous tests
        _sut.Clear();
    }

    [Test]
    public async Task InitializeRoomAsync_WhenRoomIsNew_ShouldInsertNewRoomEntityAndAddRoom()
    {
        // Arrange
        const string roomId = "newroomid";
        const string roomTitle = "New Room Title";
        var lines = new[] { $"|title|{roomTitle}", "|users|,user1,user2" };

        // Act
        await _sut.InitializeRoomAsync(roomId, lines);

        // Assert
        await using var assertContext = new BotDbContext(_dbContextOptions);
        var dbRoom = await assertContext.RoomInfo.FirstOrDefaultAsync(r => r.Id == "newroomid");

        Assert.Multiple(() =>
        {
            Assert.That(dbRoom, Is.Not.Null);
            Assert.That(dbRoom.Title, Is.EqualTo(roomTitle));
            Assert.That(_sut.HasRoom(roomId), Is.True);
        });

        _roomParameterStore.Received(1).InitializeFromRoomEntity(Arg.Is<DataAccess.Models.Room>(r => r.Id == roomId));
        await _roomParameterStore.Received(1).GetValueAsync(Parameter.Locale, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task InitializeRoomAsync_WhenRoomExists_ShouldUpdateTitleAndLoadParameters()
    {
        // Arrange
        const string roomId = "existingroom";
        const string oldTitle = "Old Title";
        const string newTitle = "New Updated Title";

        await SeedDatabaseAsync([
            new Room
            {
                Id = roomId, Title = oldTitle,
                ParameterValues = [new RoomBotParameterValue { ParameterId = "Locale", Value = "es-ES" }]
            }
        ]);

        var lines = new[] { $"|title|{newTitle}", "|users|,user1" };

        // Act
        await _sut.InitializeRoomAsync(roomId, lines);

        // Assert
        await using var assertContext = new BotDbContext(_dbContextOptions);
        var dbRoom = await assertContext.RoomInfo.FirstOrDefaultAsync(r => r.Id == roomId);

        Assert.Multiple(() =>
        {
            Assert.That(dbRoom, Is.Not.Null);
            Assert.That(dbRoom.Title, Is.EqualTo(newTitle)); // Title should be updated
            Assert.That(_sut.HasRoom(roomId), Is.True);
        });

        // Verify room was configured using existing entity
        _roomParameterStore.Received(1).InitializeFromRoomEntity(Arg.Is<Room>(r => r.Title == newTitle));
        await _roomParameterStore.Received(1).GetValueAsync(Parameter.Locale, Arg.Any<CancellationToken>());
    }

    [Test]
    public void HasRoom_WhenRoomIsInitialized_ReturnsTrue()
    {
        // Arrange
        const string roomId = "testroom";
        var room = Substitute.For<IRoom>();
        room.RoomId.Returns(roomId);

        // Use reflection or a test helper to add the room directly to the private dictionary for setup ease
        _sut.InitializeRoomAsync(roomId, ["|title|Test"]).GetAwaiter().GetResult();

        // Act & Assert
        Assert.That(_sut.HasRoom(roomId), Is.True);
    }
}