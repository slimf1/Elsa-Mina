using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public sealed class RoomsHandler : Handler
{
    private readonly IRoomsManager _roomsManager;

    public RoomsHandler(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        if (parts.Length < 2)
        {
            return Task.CompletedTask;
        }

        switch (parts[1])
        {
            case "deinit":
                _roomsManager.RemoveRoom(roomId);
                break;
            case "J":
                _roomsManager.AddUserToRoom(roomId, parts[2]);
                break;
            case "L":
                _roomsManager.RemoveUserFromRoom(roomId, parts[2]);
                break;
            case "N":
                _roomsManager.RenameUserInRoom(roomId, parts[3], parts[2]);
                break;
            case "noinit":
                var message = parts[2] switch
                {
                    "joinfailed" => "Could not join room '{0}', probably due to a lack of permissions",
                    "nonexistent" => "Room '{0}' doesn't exist, please check configuration",
                    "namerequired" => "Could not join room '{0}' because the bot is not logged in",
                    _ => string.Empty
                };
                if (!string.IsNullOrEmpty(message))
                {
                    Logger.Error(message, roomId);
                }
                break;
        }

        return Task.CompletedTask;
    }
}