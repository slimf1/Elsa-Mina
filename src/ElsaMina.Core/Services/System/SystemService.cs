using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ElsaMina.Core.Services.System;

public class SystemService : ISystemService
{
    public void Sleep(TimeSpan delay) => Thread.Sleep(delay);

    public Task SleepAsync(TimeSpan delay, CancellationToken cancellationToken = default) =>
        Task.Delay(delay, cancellationToken);

    public void Kill(int code = 1) => Environment.Exit(code);

    public SystemInfo GetSystemInfo()
    {
        var process = Process.GetCurrentProcess();
        return new SystemInfo
        {
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier,
            PagedMemory = process.PagedMemorySize64,
            WorkingSet = process.WorkingSet64,
            VirtualMemory = process.VirtualMemorySize64,
            PrivateMemory = process.PrivateMemorySize64
        };
    }
}