using ElsaMina.Core.Commands;
using Serilog;

namespace ElsaMina.Commands.Development;

public class TestParser : Parser
{
    private readonly ILogger _logger;

    public TestParser(ILogger logger)
    {
        _logger = logger;
    }

    public override Task Execute(string[] parts)
    {
        if (parts[1] == "J")
        {
            _logger.Warning("test: {user} joined", parts[2]);
        }

        return Task.CompletedTask;
    }
}