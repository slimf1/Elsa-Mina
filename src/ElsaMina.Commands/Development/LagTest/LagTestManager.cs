using System.Collections.Concurrent;
using ElsaMina.Core.Services;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.System;

namespace ElsaMina.Commands.Development.LagTest;

public class LagTestManager : ILagTestManager
{
    public const string LAG_TEST_MARKER = "⏱️ lag test...";

    private static readonly TimeSpan CANCEL_DELAY = TimeSpan.FromSeconds(10);

    private readonly IClockService _clockService;
    private readonly PendingQueryRequestsManager<string, TimeSpan> _pendingRequestsManager;

    private readonly ConcurrentDictionary<string, DateTimeOffset> _startTimes = new();

    public LagTestManager(IClockService clockService, ISystemService systemService)
    {
        _clockService = clockService;
        _pendingRequestsManager = new PendingQueryRequestsManager<string, TimeSpan>(
            systemService,
            CANCEL_DELAY,
            () => TimeSpan.MinValue);
    }

    public Task<TimeSpan> StartLagTestAsync(string roomId, CancellationToken cancellationToken = default)
    {
        _startTimes[roomId] = _clockService.CurrentUtcDateTimeOffset;
        return _pendingRequestsManager.AddOrReplace(roomId, cancellationToken);
    }

    public void HandleEcho(string roomId)
    {
        if (!_startTimes.TryRemove(roomId, out var startTime))
        {
            return;
        }

        var elapsed = _clockService.CurrentUtcDateTimeOffset - startTime;
        _pendingRequestsManager.TryResolve(roomId, elapsed);
    }
}
