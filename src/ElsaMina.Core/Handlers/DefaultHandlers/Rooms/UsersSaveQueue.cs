using System.Collections.Concurrent;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using ElsaMina.Logging;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Core.Handlers.DefaultHandlers.Rooms;

public sealed class UserSaveQueue : IUserSaveQueue
{
    private readonly ConcurrentDictionary<string, string> _pendingUsers = new();
    private readonly IBotDbContextFactory _dbContextFactory;
    private readonly PeriodicTimer _timer;
    private readonly int _batchSize;

    private readonly SemaphoreSlim _flushLock = new(1, 1);
    private readonly Lock _flushTaskLock = new();

    private Task _currentFlushTask;
    private readonly CancellationTokenSource _cts = new();

    public UserSaveQueue(
        IBotDbContextFactory dbContextFactory,
        IConfiguration configuration)
    {
        _dbContextFactory = dbContextFactory;
        _batchSize = configuration.UserUpdateBatchSize;
        _timer = new PeriodicTimer(configuration.UserUpdateFlushInterval);

        _ = Task.Run(BackgroundFlushAsync);
    }

    public void Enqueue(string userName)
    {
        var userId = userName.ToLowerAlphaNum();
        _pendingUsers[userId] = userName;

        if (_pendingUsers.Count >= _batchSize)
        {
            _ = FlushAsync(CancellationToken.None);
        }
    }

    private async Task BackgroundFlushAsync()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                await FlushAsync(_cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public Task FlushAsync(CancellationToken cancellationToken)
    {
        lock (_flushTaskLock)
        {
            if (_currentFlushTask?.IsCompleted == false)
            {
                return _currentFlushTask;
            }

            _currentFlushTask = FlushInternalAsync(cancellationToken).ContinueWith(task =>
                {
                    lock (_flushTaskLock)
                    {
                        _currentFlushTask = null;
                    }

                    return task;
                }, TaskScheduler.Default)
                .Unwrap();

            return _currentFlushTask;
        }
    }

    private async Task FlushInternalAsync(CancellationToken cancellationToken)
    {
        await _flushLock.WaitAsync(cancellationToken);
        try
        {
            while (!_pendingUsers.IsEmpty)
            {
                var batch = _pendingUsers.ToArray();
                _pendingUsers.Clear();

                await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

                var userIds = batch.Select(kvp => kvp.Key).ToList();

                var existingUsers = await dbContext.Users
                    .Where(user => userIds.Contains(user.UserId))
                    .ToDictionaryAsync(u => u.UserId, cancellationToken);

                Log.Debug("Saving user ids : {0}", string.Join(", ", userIds));
                foreach (var (userId, rawUserName) in batch)
                {
                    var userName = rawUserName[1..];

                    if (existingUsers.TryGetValue(userId, out var user))
                    {
                        if (user.UserName != userName)
                        {
                            user.UserName = userName;
                        }
                    }
                    else
                    {
                        dbContext.Users.Add(new SavedUser
                        {
                            UserId = userId,
                            UserName = userName
                        });
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        finally
        {
            _flushLock.Release();
        }
    }

    public async Task WaitForFlushAsync(CancellationToken cancellationToken = default)
    {
        Task flushTask;

        lock (_flushTaskLock)
        {
            flushTask = _currentFlushTask;
        }

        if (flushTask != null)
        {
            await flushTask.WaitAsync(cancellationToken);
        }
    }

    public async Task AcquireLockAsync(CancellationToken cancellationToken = default)
    {
        await _flushLock.WaitAsync(cancellationToken);
    }

    public void ReleaseLock()
    {
        _flushLock.Release();
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        _timer.Dispose();
        await FlushAsync(CancellationToken.None);
        await WaitForFlushAsync(CancellationToken.None);
    }
}