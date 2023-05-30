using ElsaMina.Core.Client;
using Newtonsoft.Json;
using Serilog;

namespace ElsaMina.Core.Services.UserData;

public class UserDataManager : IUserDataManager
{
    private readonly ILogger _logger;
    private readonly IClient _client;

    private TaskCompletionSource<UserDataDto> _taskCompletionSource;

    public UserDataManager(ILogger logger, IClient client)
    {
        _logger = logger;
        _client = client;
    }

    public Task<UserDataDto> GetUserData(string userId)
    {
        _client.Send($"|/cmd userdetails {userId}");
        _taskCompletionSource = new TaskCompletionSource<UserDataDto>();
        return _taskCompletionSource.Task;
    }

    public void HandleReceivedUserData(string message)
    {
        if (_taskCompletionSource == null)
        {
            return;
        }

        UserDataDto userDataDto = null;
        try
        {
            userDataDto = JsonConvert.DeserializeObject<UserDataDto>(message);
        }
        catch (JsonSerializationException exception)
        {
            _logger.Error(exception, "Error while deserializing userdata json");
        }

        _taskCompletionSource.TrySetResult(userDataDto);
        _taskCompletionSource = null;
    }
}