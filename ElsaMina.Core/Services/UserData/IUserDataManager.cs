namespace ElsaMina.Core.Services.UserData;

public interface IUserDataManager
{
    Task<UserDataDto> GetUserData(string userId);
    void HandleReceivedUserData(string message);
}