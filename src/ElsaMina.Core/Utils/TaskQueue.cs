using System.Collections.Concurrent;

namespace ElsaMina.Core.Utils;

public class TaskQueue
{
    private readonly ConcurrentQueue<Func<Task>> _taskQueue = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _isProcessing;

    public void Enqueue(Func<Task> taskFunc)
    {
        _taskQueue.Enqueue(taskFunc);
        _ = ProcessQueueAsync();
    }

    private async Task ProcessQueueAsync()
    {
        if (!_isProcessing)
        {
            _isProcessing = true;

            while (_taskQueue.TryDequeue(out var taskFunc))
            {
                await _semaphore.WaitAsync();
                try
                {
                    await taskFunc();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error processing task in queue");
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            _isProcessing = false;
        }
    }
}