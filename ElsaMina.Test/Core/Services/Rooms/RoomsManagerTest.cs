using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using FluentAssertions;
using NSubstitute;
using Serilog;

namespace ElsaMina.Test.Core.Services.Rooms;

public class RoomsManagerTest
{
    private ILogger _logger;
    private IConfigurationService _configurationService;

    private RoomsManager _roomsManager;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
        _configurationService = Substitute.For<IConfigurationService>();

        _roomsManager = new RoomsManager(_logger, _configurationService);
    }

    private void InitializeFakeRooms()
    {
        const string roomId1 = "my-room";
        const string roomTitle1 = "My Room";
        var roomUsers1 = new List<string> { "&Test", "+James@!", " Dude" };
        
        const string roomId2 = "franais";
        const string roomTitle2 = "Français";
        var roomUsers2 = new List<string> { "&Teclis", "!Lionyx", "@Earth", " Mec" };
        
        _roomsManager.InitializeRoom(roomId1, roomTitle1, roomUsers1);
        _roomsManager.InitializeRoom(roomId2, roomTitle2, roomUsers2);
    }
    

    [Test]
    public void Test_InitializeRoom_ShouldAddRoom()
    {
        // Arrange & Act
        InitializeFakeRooms();
        
        // Assert
        _roomsManager.HasRoom("my-room").Should().BeTrue();
        _roomsManager.HasRoom("franais").Should().BeTrue();
        _roomsManager.HasRoom("doesn't exist").Should().BeFalse();
        _roomsManager.GetRoom("my-room").Should().NotBeNull();
        _roomsManager.GetRoom("franais").Should().NotBeNull();
        _roomsManager.GetRoom("doesn't exist").Should().BeNull();
        
        _roomsManager.GetRoom("my-room").RoomId.Should().Be("my-room");
        _roomsManager.GetRoom("my-room").Name.Should().Be("My Room");
        _roomsManager.GetRoom("my-room").Users.Count.Should().Be(3);
        _roomsManager.GetRoom("my-room").Users.ContainsKey("test").Should().BeTrue();
        _roomsManager.GetRoom("my-room").Users["test"].IsIdle.Should().BeFalse();
        _roomsManager.GetRoom("my-room").Users.ContainsKey("james").Should().BeTrue();
        _roomsManager.GetRoom("my-room").Users["james"].IsIdle.Should().BeTrue();
        _roomsManager.GetRoom("my-room").Users.ContainsKey("dude").Should().BeTrue();
        _roomsManager.GetRoom("my-room").Users["dude"].IsIdle.Should().BeFalse();

        _roomsManager.GetRoom("franais").RoomId.Should().Be("franais");
        _roomsManager.GetRoom("franais").Name.Should().Be("Français");
        _roomsManager.GetRoom("franais").Users.Count.Should().Be(4);
        _roomsManager.GetRoom("franais").Users.ContainsKey("teclis").Should().BeTrue();
        _roomsManager.GetRoom("franais").Users.ContainsKey("lionyx").Should().BeTrue();
        _roomsManager.GetRoom("franais").Users.ContainsKey("earth").Should().BeTrue();
        _roomsManager.GetRoom("franais").Users.ContainsKey("mec").Should().BeTrue();
    }

    [Test]
    public void Test_AddUserToRoom_ShouldAddUserToRoom_WhenRoomExists()
    {
        // Arrange
        InitializeFakeRooms();
        
        // Act
        _roomsManager.AddUserToRoom("franais", "%Polaire");
        
        // Assert
        _roomsManager.GetRoom("franais").Users.Count.Should().Be(5);
        _roomsManager.GetRoom("franais").Users.ContainsKey("polaire").Should().BeTrue();
    }
    
    [Test]
    public void Test_AddUserToRoom_ShouldDoNothingWhenRoomDoesntExist()
    {
        // Arrange
        InitializeFakeRooms();
        
        // Act
        _roomsManager.AddUserToRoom("espaol", "&speks");
        
        // Assert
        _roomsManager.GetRoom("espaol").Should().BeNull();
        _roomsManager.GetRoom("franais").Users.Count.Should().Be(4);
    }
    
    [Test]
    public void Test_RemoveUserFromRoom_ShouldRemoveUserFromRoom_WhenRoomAndUserExist()
    {
        // Arrange
        InitializeFakeRooms();
        
        // Act
        _roomsManager.RemoveUserFromRoom("franais", "@Earth");
        
        // Assert
        _roomsManager.GetRoom("franais").Users.Count.Should().Be(3);
        _roomsManager.GetRoom("franais").Users.ContainsKey("earth").Should().BeFalse();
    }
    
    [Test]
    public void Test_RemoveUserFromRoom_ShouldDoNothing_WhenRoomExistsButUserDoesnt()
    {
        // Arrange
        InitializeFakeRooms();
        
        // Act
        _roomsManager.RemoveUserFromRoom("franais", "+Corentin");
        
        // Assert
        _roomsManager.GetRoom("franais").Users.Count.Should().Be(4);
        _roomsManager.GetRoom("franais").Users.ContainsKey("corentin").Should().BeFalse();
    }
    
    [Test]
    public void Test_RemoveUserFromRoom_ShouldDoNothing_WhenRoomDoesntExist()
    {
        // Arrange
        InitializeFakeRooms();
        
        // Act
        _roomsManager.RemoveUserFromRoom("espaol", "@Earth");
        
        // Assert
        _roomsManager.GetRoom("franais").Users.Count.Should().Be(4);
        _roomsManager.GetRoom("franais").Users.ContainsKey("earth").Should().BeTrue();
    }
    
    [Test]
    public void Test_RenameUserInRoom_ShouldRenameUser()
    {
        // Arrange
        InitializeFakeRooms();
        
        // Act
        _roomsManager.RenameUserInRoom("franais", "mec", "&DieuSupreme");
        
        // Assert
        _roomsManager.GetRoom("franais").Users.Count.Should().Be(4);
        _roomsManager.GetRoom("franais").Users.ContainsKey("mec").Should().BeFalse();
        _roomsManager.GetRoom("franais").Users.ContainsKey("dieusupreme").Should().BeTrue();
        _roomsManager.GetRoom("franais").Users["dieusupreme"].Name.Should().Be("DieuSupreme");
        _roomsManager.GetRoom("franais").Users["dieusupreme"].Rank.Should().Be('&');
        _roomsManager.GetRoom("franais").Users["dieusupreme"].Rank.Should().Be('&');
    }
    
    [Test]
    public void Test_RenameUserInRoom_ShouldRenameUser_WhenUserIsAfk()
    {
        // Arrange
        InitializeFakeRooms();
        
        // Act
        _roomsManager.RenameUserInRoom("franais", "teclis", "&Teclis@!");
        
        // Assert
        _roomsManager.GetRoom("franais").Users.Count.Should().Be(4);
        _roomsManager.GetRoom("franais").Users["teclis"].IsIdle.Should().BeTrue();
    }
    
    [Test]
    public void Test_RenameUserInRoom_ShouldRenameUser_WhenUserIsNotAfkAnymore()
    {
        // Arrange
        InitializeFakeRooms();
        
        // Act
        _roomsManager.RenameUserInRoom("my-room", "james", "+James");
        
        // Assert
        _roomsManager.GetRoom("my-room").Users.Count.Should().Be(3);
        _roomsManager.GetRoom("my-room").Users["james"].IsIdle.Should().BeFalse();
    }

}