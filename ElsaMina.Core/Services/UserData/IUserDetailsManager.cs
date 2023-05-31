namespace ElsaMina.Core.Services.UserData;

public interface IUserDetailsManager
{
    Task<UserDetailsDto> GetUserDetails(string userId);
    void HandleReceivedUserDetails(string message);
}