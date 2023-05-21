using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Serilog;

namespace ElsaMina.Commands.Development;

public class Script : ICommand
{
    public static string Name => "script";
    public bool IsAllowedInPm => true;
    public bool IsWhitelistOnly => true;
    public bool IsHidden => true;

    private readonly ILogger _logger;
    private readonly IDependencyContainerService _dependencyContainerService;

    public Script(ILogger logger, IDependencyContainerService dependencyContainerService)
    {
        _logger = logger;
        _dependencyContainerService = dependencyContainerService;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public class Globals
    {
        public IContext Context;
        public IDependencyContainerService Container;
    }
    
    public async Task Run(IContext context)
    {
        try
        {
            var options = ScriptOptions.Default.WithReferences(typeof(Core.Bot.Bot).Assembly)
                .WithReferences(typeof(Script).Assembly)
                .WithImports("ElsaMina.Core");
            var globals = new Globals { Context = context, Container = _dependencyContainerService };
            var result = await CSharpScript.EvaluateAsync(context.Target, options, globals: globals);
            if (result != null)
            {
                context.Reply(result.ToString());
            }
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occured while using script command.");
            context.Reply($"An error occured: {exception.Message}");
        }
    }
}