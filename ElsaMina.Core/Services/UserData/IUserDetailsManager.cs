namespace ElsaMina.Core.Services.UserData;

public interface IUserDetailsManager
{
    Task<UserDataDto> GetUserDetails(string userId);
    void HandleReceivedUserDetails(string message);
}