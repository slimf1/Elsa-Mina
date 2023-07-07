using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Utils;
using Serilog;

namespace ElsaMina.Core.Services.UserData;

public class UserDataService : IUserDataService
{
    private const string USER_DATA_URL = "https://pokemonshowdown.com/users/{0}.json";

    private readonly Dictionary<string, UserDataDto> _userDataCache = new();

    private readonly ILogger _logger;
    private readonly IHttpService _httpService;

    public UserDataService(ILogger logger, IHttpService httpService)
    {
        _logger = logger;
        _httpService = httpService;
    }

    public async Task<UserDataDto> GetUserData(string userName)
    {
        var userId = userName.ToLowerAlphaNum();
        if (_userDataCache.TryGetValue(userId, out var cachedUserData))
        {
            return cachedUserData;
        }
        var uri = string.Format(USER_DATA_URL, userId);
        try
        {
            var userData = await _httpService.Get<UserDataDto>(uri);
            _userDataCache[userId] = userData;
            return userData;
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "Error while getting user data");
            return null;
        }
    }

    public async Task<DateTimeOffset> GetRegisterDate(string userName)
    {
        var userData = await GetUserData(userName);
        return userData == null
            ? DateTimeOffset.MinValue
            : DateTimeOffset.FromUnixTimeSeconds(userData.RegisterTime);
    }
}