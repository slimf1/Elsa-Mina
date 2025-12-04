using System.Collections.Concurrent;
using ElsaMina.Core.Services.System;
using ElsaMina.Logging;
using Newtonsoft.Json;

namespace ElsaMina.Core.Services.UserDetails;

public class UserDetailsManager : IUserDetailsManager
{
    private static readonly TimeSpan CANCEL_DELAY = TimeSpan.FromSeconds(5);

    private readonly IClient _client;
    private readonly ISystemService _systemService;

    private readonly ConcurrentDictionary<string, (TaskCompletionSource<UserDetailsDto> Tcs, CancellationTokenSource TimeoutCts)>
        _pendingRequests = new();

    public UserDetailsManager(IClient client, ISystemService systemService)
    {
        _client = client;
        _systemService = systemService;
    }

    public Task<UserDetailsDto> GetUserDetailsAsync(string userId, CancellationToken cancellationToken = default)
    {
        _client.Send($"|/cmd userdetails {userId}");

        if (_pendingRequests.TryRemove(userId, out var oldEntry))
        {
            oldEntry.TimeoutCts.Cancel();
            oldEntry.Tcs.TrySetCanceled();
        }

        var tcs = new TaskCompletionSource<UserDetailsDto>(TaskCreationOptions.RunContinuationsAsynchronously);
        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _pendingRequests[userId] = (tcs, timeoutCts);

        _ = RunTimeoutAsync(userId, timeoutCts.Token);

        return tcs.Task;
    }

    private async Task RunTimeoutAsync(string userId, CancellationToken cancellationToken)
    {
        try
        {
            await _systemService.SleepAsync(CANCEL_DELAY, cancellationToken);
            if (_pendingRequests.TryRemove(userId, out var entry))
            {
                entry.Tcs.TrySetResult(null);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal: cancelled because data arrived or request was replaced
        }
    }

    public void HandleReceivedUserDetails(string message)
    {
        UserDetailsDto dto = null;

        try
        {
            // Handle servers returning "rooms: false" instead of {}
            message = message.Replace("\"rooms\":false", "\"rooms\":{}")
                             .Replace("\"rooms\": false", "\"rooms\":{}");

            dto = JsonConvert.DeserializeObject<UserDetailsDto>(message);
        }
        catch (JsonSerializationException ex)
        {
            Log.Error(ex, "Error while deserializing userdata json");
        }

        if (dto == null)
            return;

        if (_pendingRequests.TryRemove(dto.UserId, out var entry))
        {
            entry.TimeoutCts.Cancel(); // stop timeout
            entry.Tcs.TrySetResult(dto);
        }
    }
}
