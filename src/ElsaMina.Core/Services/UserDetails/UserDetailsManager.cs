using ElsaMina.Core.Client;
using Newtonsoft.Json;
using Serilog;

namespace ElsaMina.Core.Services.UserDetails;

public class UserDetailsManager : IUserDetailsManager
{
    private const int CANCEL_DELAY = 5000;

    private readonly ILogger _logger;
    private readonly IClient _client;

    private readonly Dictionary<string, TaskCompletionSource<UserDetailsDto>> _taskCompletionSources = new();

    public UserDetailsManager(ILogger logger, IClient client)
    {
        _logger = logger;
        _client = client;
    }

    public Task<UserDetailsDto> GetUserDetails(string userId)
    {
        _client.Send($"|/cmd userdetails {userId}");
        if (_taskCompletionSources.TryGetValue(userId, out var taskCompletionSource))
        {
            taskCompletionSource.TrySetResult(null);
            _taskCompletionSources[userId] = null;
            _taskCompletionSources.Remove(userId);
        }

        _taskCompletionSources[userId] = new TaskCompletionSource<UserDetailsDto>();
        Task.Run(async () =>
        {
            await Task.Delay(CANCEL_DELAY);
            if (!_taskCompletionSources.ContainsKey(userId))
            {
                return;
            }

            _taskCompletionSources[userId].TrySetResult(null);
            _taskCompletionSources[userId] = null;
            _taskCompletionSources.Remove(userId);
        });
        return _taskCompletionSources[userId].Task;
    }

    public void HandleReceivedUserDetails(string message)
    {
        UserDetailsDto userDetailsDto = null;
        try
        {
            // Ugly workaround for the userdetails json respsonse
            // When user has no rooms => rooms is false, otherwise it's a dictionary 🤡
            message = message.Replace("\"rooms\":false", "\"rooms\":{}");
            message = message.Replace("\"rooms\": false", "\"rooms\":{}");
            userDetailsDto = JsonConvert.DeserializeObject<UserDetailsDto>(message);
        }
        catch (JsonSerializationException exception)
        {
            _logger.Error(exception, "Error while deserializing userdata json");
        }

        if (userDetailsDto == null)
        {
            return;
        }

        if (!_taskCompletionSources.TryGetValue(userDetailsDto.UserId, out var taskCompletionSource))
        {
            return;
        }

        taskCompletionSource.TrySetResult(userDetailsDto);
        _taskCompletionSources[userDetailsDto.UserId] = null;
        _taskCompletionSources.Remove(userDetailsDto.UserId);
    }
}