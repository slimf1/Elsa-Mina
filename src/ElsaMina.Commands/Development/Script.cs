using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ElsaMina.Commands.Development;

[NamedCommand("script")]
public class Script : DevelopmentCommand
{
    private readonly IDependencyContainerService _dependencyContainerService;

    public Script(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public class Globals
    {
        public IContext Context { get; set; }
        public IDependencyContainerService Container { get; set; }
    }

    public override async Task Run(IContext context)
    {
        try
        {
            var options = ScriptOptions.Default.WithReferences(typeof(Bot).Assembly)
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
            Log.Error(exception, "An error occurred while using script command.");
            context.Reply($"An error occurred: {exception.Message}");
        }
    }
}