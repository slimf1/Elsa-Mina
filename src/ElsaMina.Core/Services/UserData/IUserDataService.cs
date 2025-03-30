namespace ElsaMina.Core.Services.UserData;

public interface IUserDataService
{
    Task<UserDataDto> GetUserData(string userName, CancellationToken cancellationToken = default);
    Task<DateTimeOffset> GetRegisterDateAsync(string userName, CancellationToken cancellationToken = default);
}