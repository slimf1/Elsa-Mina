namespace ElsaMina.Core.Services.UserData;

public interface IUserDataService
{
    Task<UserDataDto> GetUserData(string userName);
    Task<DateTimeOffset> GetRegisterDateAsync(string userName);
}