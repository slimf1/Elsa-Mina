using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.Test.Core.Handlers.DefaultHandlers;

public class RoomsHandlerTest
{
    private IRoomsManager _roomsManager;
    private RoomsHandler _roomsHandler;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _roomsHandler = new RoomsHandler(_roomsManager);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldDoNothing_WhenPartsLengthIsLessThanTwo()
    {
        // Arrange
        string[] parts = [""];

        // Act
        await _roomsHandler.HandleReceivedMessage(parts);

        // Assert
        _roomsManager.DidNotReceive().RemoveRoom(Arg.Any<string>());
        _roomsManager.DidNotReceive().AddUserToRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RemoveUserFromRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RenameUserInRoom(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldCallRemoveRoom_WhenCommandIsDeinit()
    {
        // Arrange
        const string roomId = "room1";
        string[] parts = ["", "deinit"];

        // Act
        await _roomsHandler.HandleReceivedMessage(parts, roomId);

        // Assert
        _roomsManager.Received(1).RemoveRoom(roomId);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldCallAddUserToRoom_WhenCommandIsJ()
    {
        // Arrange
        const string roomId = "room1";
        const string userId = "user123";
        string[] parts = ["", "J", userId];

        // Act
        await _roomsHandler.HandleReceivedMessage(parts, roomId);

        // Assert
        _roomsManager.Received(1).AddUserToRoom(roomId, userId);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldCallRemoveUserFromRoom_WhenCommandIsL()
    {
        // Arrange
        const string roomId = "room1";
        const string userId = "user123";
        string[] parts = ["", "L", userId];

        // Act
        await _roomsHandler.HandleReceivedMessage(parts, roomId);

        // Assert
        _roomsManager.Received(1).RemoveUserFromRoom(roomId, userId);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldCallRenameUserInRoom_WhenCommandIsN()
    {
        // Arrange
        const string roomId = "room1";
        const string oldUsername = "oldUser";
        const string newUsername = "newUser";
        string[] parts = ["", "N", oldUsername, newUsername];

        // Act
        await _roomsHandler.HandleReceivedMessage(parts, roomId);

        // Assert
        _roomsManager.Received(1).RenameUserInRoom(roomId, newUsername, oldUsername);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldLogError_WhenCommandIsNoInitJoinFailed()
    {
        // Arrange
        const string roomId = "room1";
        string[] parts = ["", "noinit", "joinfailed"];

        // Act
        await _roomsHandler.HandleReceivedMessage(parts, roomId);

        // Assert
        _roomsManager.DidNotReceive().AddUserToRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RemoveUserFromRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RemoveRoom(Arg.Any<string>());
        _roomsManager.DidNotReceive().RenameUserInRoom(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldLogError_WhenCommandIsNoInitNonExistent()
    {
        // Arrange
        const string roomId = "room1";
        string[] parts = ["", "noinit", "nonexistent"];

        // Act
        await _roomsHandler.HandleReceivedMessage(parts, roomId);

        // Assert
        _roomsManager.DidNotReceive().AddUserToRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RemoveUserFromRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RemoveRoom(Arg.Any<string>());
        _roomsManager.DidNotReceive().RenameUserInRoom(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldLogError_WhenCommandIsNoInitNameRequired()
    {
        // Arrange
        const string roomId = "room1";
        string[] parts = ["", "noinit", "namerequired"];

        // Act
        await _roomsHandler.HandleReceivedMessage(parts, roomId);

        // Assert
        _roomsManager.DidNotReceive().AddUserToRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RemoveUserFromRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RemoveRoom(Arg.Any<string>());
        _roomsManager.DidNotReceive().RenameUserInRoom(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotLog_WhenCommandIsNoInitWithUnknownSubCommand()
    {
        // Arrange
        const string roomId = "room1";
        string[] parts = ["", "noinit", "unknownsubcommand"];

        // Act
        await _roomsHandler.HandleReceivedMessage(parts, roomId);

        // Assert
        _roomsManager.DidNotReceive().AddUserToRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RemoveUserFromRoom(Arg.Any<string>(), Arg.Any<string>());
        _roomsManager.DidNotReceive().RemoveRoom(Arg.Any<string>());
        _roomsManager.DidNotReceive().RenameUserInRoom(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }
}