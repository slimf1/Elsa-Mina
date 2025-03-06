namespace ElsaMina.Core.Services.UserDetails;

public interface IUserDetailsManager
{
    Task<UserDetailsDto> GetUserDetailsAsync(string userId, CancellationToken cancellationToken = default);
    void HandleReceivedUserDetails(string message);
}