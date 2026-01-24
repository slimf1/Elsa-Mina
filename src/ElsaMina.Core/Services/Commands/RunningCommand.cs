using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Commands;

public sealed record RunningCommand(
    Guid ExecutionId,
    string CommandName,
    IContext Context,
    CancellationTokenSource CancellationTokenSource,
    Task Task);