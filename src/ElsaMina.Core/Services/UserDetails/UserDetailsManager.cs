using System.Collections.Concurrent;
using ElsaMina.Core.Services.System;
using Newtonsoft.Json;

namespace ElsaMina.Core.Services.UserDetails;

public class UserDetailsManager : IUserDetailsManager
{
    private static readonly TimeSpan CANCEL_DELAY = TimeSpan.FromSeconds(5);

    private readonly IClient _client;
    private readonly ISystemService _systemService;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<UserDetailsDto>> _taskCompletionSources = new();

    public UserDetailsManager(IClient client,
        ISystemService systemService)
    {
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
            Logger.Current.Error(exception, "Error while deserializing userdata json");
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