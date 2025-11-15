using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Services.System;

public class SystemInfo
{
    public long WorkingSet { get; set; }
    public long PrivateMemory { get; set; }
    public long VirtualMemory { get; set; }
    public long PagedMemory { get; set; }
    public string FrameworkDescription { get; set; }
    public string RuntimeIdentifier { get; set; }
    
    public override string ToString()
    {
        return
            $"{FrameworkDescription} | {RuntimeIdentifier} | " +
            $"WS: {WorkingSet.ToReadableDataSize()}, " +
            $"Priv: {PrivateMemory.ToReadableDataSize()}, " +
            $"Paged: {PagedMemory.ToReadableDataSize()}, " +
            $"Virt: {VirtualMemory.ToReadableDataSize()}";
    }
}