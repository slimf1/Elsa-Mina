namespace ElsaMina.Core.Services.UserDetails;

public interface IUserDetailsManager
{
    Task<UserDetailsDto> GetUserDetails(string userId);
    void HandleReceivedUserDetails(string message);
}