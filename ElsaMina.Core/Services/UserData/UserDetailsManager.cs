using ElsaMina.Core.Client;
using Newtonsoft.Json;
using Serilog;

namespace ElsaMina.Core.Services.UserData;

public class UserDetailsManager : IUserDetailsManager
{
    private const int CANCEL_DELAY = 3000;
    
    private readonly ILogger _logger;
    private readonly IClient _client;

    private readonly Dictionary<string, TaskCompletionSource<UserDataDto>> _taskCompletionSources = new();

    public UserDetailsManager(ILogger logger, IClient client)
    {
        _logger = logger;
        _client = client;
    }

    public Task<UserDataDto> GetUserDetails(string userId)
    {
        _client.Send($"|/cmd userdetails {userId}");
        if (_taskCompletionSources.TryGetValue(userId, out var taskCompletionSource))
        {
            taskCompletionSource.TrySetResult(null);
            _taskCompletionSources[userId] = null;
            _taskCompletionSources.Remove(userId);
        }
        _taskCompletionSources[userId] = new TaskCompletionSource<UserDataDto>();
        Task.Run(async () =>
        {
            await Task.Delay(CANCEL_DELAY);
            if (!_taskCompletionSources.ContainsKey(userId))
            {
                return;
            }

            _taskCompletionSources[userId].TrySetResult(null);
            _taskCompletionSources.Remove(userId);
        });
        return _taskCompletionSources[userId].Task;
    }

    public void HandleReceivedUserDetails(string message)
    {
        UserDataDto userDataDto = null;
        try
        {
            userDataDto = JsonConvert.DeserializeObject<UserDataDto>(message);
        }
        catch (JsonSerializationException exception)
        {
            _logger.Error(exception, "Error while deserializing userdata json");
        }

        if (userDataDto == null)
        {
            return;
        }

        if (!_taskCompletionSources.TryGetValue(userDataDto.UserId, out var taskCompletionSource))
        {
            return;
        }
        
        taskCompletionSource.TrySetResult(userDataDto);
        _taskCompletionSources[userDataDto.UserId] = null;
        _taskCompletionSources.Remove(userDataDto.UserId);
    }
}