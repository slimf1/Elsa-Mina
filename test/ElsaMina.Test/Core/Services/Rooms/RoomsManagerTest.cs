﻿using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Core.Services.Rooms;

public class RoomsManagerTest
{
    private IConfigurationManager _configurationManager;
    private IRoomParametersRepository _roomParametersRepository;
    private IRoomConfigurationParametersFactory _roomConfigurationParametersFactory;
    private IRoomBotParameterValueRepository _roomBotParameterValueRepository;

    private RoomsManager _roomsManager;

    [SetUp]
    public void SetUp()
    {
        _configurationManager = Substitute.For<IConfigurationManager>();
        _roomParametersRepository = Substitute.For<IRoomParametersRepository>();
        _roomConfigurationParametersFactory = Substitute.For<IRoomConfigurationParametersFactory>();
        _roomBotParameterValueRepository = Substitute.For<IRoomBotParameterValueRepository>();

        _roomsManager = new RoomsManager(_configurationManager, _roomConfigurationParametersFactory,
            _roomParametersRepository, _roomBotParameterValueRepository);
    }

    private async Task InitializeFakeRooms()
    {
        const string roomId1 = "my-room";
        const string roomTitle1 = "My Room";
        var roomUsers1 = new List<string> { "&Test", "+James@!", " Dude" };

        const string roomId2 = "franais";
        const string roomTitle2 = "Français";
        var roomUsers2 = new List<string> { "&Teclis", "!Lionyx", "@Earth", " Mec" };

        await _roomsManager.InitializeRoom(roomId1, roomTitle1, roomUsers1);
        await _roomsManager.InitializeRoom(roomId2, roomTitle2, roomUsers2);
    }

    [Test]
    public async Task Test_InitializeRoom_ShouldUseDefaultLocale_WhenRoomParametersDoesntExist()
    {
        // Arrange
        _configurationManager.Configuration.Returns(new Configuration
        {
            DefaultLocaleCode = "zh-CN"
        });
        _roomParametersRepository.GetByIdAsync("franais").ReturnsNull();

        // Act
        await _roomsManager.InitializeRoom("franais", "Français", Enumerable.Empty<string>());

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Culture.Name, Is.EqualTo("zh-CN"));
    }
    
    [Test]
    public async Task Test_InitializeRoom_ShouldUserLocaleStoredInDb_WhenRoomParametersExist()
    {
        // Arrange
        _roomParametersRepository.GetByIdAsync("franais").Returns(new RoomParameters
        {
            Id = "franais",
            ParameterValues = new List<RoomBotParameterValue>
            {
                new()
                {
                    ParameterId = RoomParametersConstants.LOCALE,
                    Value = "fr-FR"
                }
            }
        });

        // Act
        await _roomsManager.InitializeRoom("franais", "Français", Enumerable.Empty<string>());

        // Assert
        Assert.That(_roomsManager.GetRoom("franais").Culture.Name, Is.EqualTo("fr-FR"));
    }

    [Test]
    public async Task Test_InitializeRoom_ShouldAddRoom()
    {
        // Arrange & Act
        await InitializeFakeRooms();

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

        // Assert
        Assert.That(_roomsManager.GetRoom("espaol"), Is.Null);
        Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(4));
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
        Assert.That(_roomsManager.GetRoom("franais").Users, Has.Count.EqualTo(4));
        Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("mec"), Is.False);
        Assert.That(_roomsManager.GetRoom("franais").Users.ContainsKey("dieusupreme"), Is.True);
        Assert.That(_roomsManager.GetRoom("franais").Users["dieusupreme"].Name, Is.EqualTo("DieuSupreme"));
        Assert.That(_roomsManager.GetRoom("franais").Users["dieusupreme"].Rank, Is.EqualTo('&'));
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
}