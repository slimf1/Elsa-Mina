using System.Collections.Concurrent;
using ElsaMina.Core.Services.System;
using Newtonsoft.Json;
using Serilog;

namespace ElsaMina.Core.Services.UserDetails;

public class UserDetailsManager : IUserDetailsManager
{
    private const int CANCEL_DELAY = 5000;

    private readonly ILogger _logger;
    private readonly IClient _client;
    private readonly ISystemService _systemService;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<UserDetailsDto>> _taskCompletionSources = new();

    public UserDetailsManager(ILogger logger,
        IClient client,
        ISystemService systemService)
    {
        _logger = logger;
        _client = client;
        _systemService = systemService;
    }

    public Task<UserDetailsDto> GetUserDetails(string userId)
    {
        _client.Send($"|/cmd userdetails {userId}");
        if (_taskCompletionSources.TryGetValue(userId, out var taskCompletionSource))
        {
            taskCompletionSource.SetResult(null);
            _taskCompletionSources.Remove(userId, out _);
        }

        _taskCompletionSources[userId] = new TaskCompletionSource<UserDetailsDto>();
        Task.Run(async () =>
        {
            await _systemService.SleepAsync(CANCEL_DELAY);
            if (!_taskCompletionSources.TryGetValue(userId, out var tcs))
            {
                return;
            }

            tcs.SetResult(null);
            _taskCompletionSources.Remove(userId, out _);
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

        taskCompletionSource.SetResult(userDetailsDto);
        _taskCompletionSources.Remove(userDetailsDto.UserId, out _);
    }
}