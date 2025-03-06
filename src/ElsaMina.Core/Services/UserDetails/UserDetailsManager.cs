using System.Collections.Concurrent;
using ElsaMina.Core.Services.System;
using Newtonsoft.Json;

namespace ElsaMina.Core.Services.UserDetails;

public class UserDetailsManager : IUserDetailsManager
{
    private static readonly TimeSpan CANCEL_DELAY = TimeSpan.FromSeconds(5);

    private readonly IClient _client;
    private readonly ISystemService _systemService;
    private readonly
        ConcurrentDictionary<string, (TaskCompletionSource<UserDetailsDto> Tcs, CancellationTokenSource Cts)>
        _taskCompletionSources = new();

    public UserDetailsManager(IClient client, ISystemService systemService)
    {
        _client = client;
        _systemService = systemService;
    }

    public Task<UserDetailsDto> GetUserDetailsAsync(string userId, CancellationToken cancellationToken = default)
    {
        _client.Send($"|/cmd userdetails {userId}");

        if (_taskCompletionSources.TryGetValue(userId, out var existingEntry))
        {
            existingEntry.Cts.Cancel();
            _taskCompletionSources.TryRemove(userId, out _);
        }

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var tcs = new TaskCompletionSource<UserDetailsDto>(TaskCreationOptions.RunContinuationsAsynchronously);

        _taskCompletionSources[userId] = (tcs, cts);

        _ = Task.Run(async () =>
        {
            try
            {
                await _systemService.SleepAsync(CANCEL_DELAY, cts.Token);
                if (_taskCompletionSources.TryRemove(userId, out var entry))
                {
                    entry.Tcs.TrySetResult(null);
                }
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }
        }, cts.Token);

        return tcs.Task;
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
            Logger.Error(exception, "Error while deserializing userdata json");
        }

        if (userDetailsDto == null || !_taskCompletionSources.TryRemove(userDetailsDto.UserId, out var entry))
        {
            return;
        }

        entry.Cts.Cancel(); // Cancel the timeout task
        entry.Tcs.TrySetResult(userDetailsDto);
    }
}