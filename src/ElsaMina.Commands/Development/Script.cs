using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ElsaMina.Commands.Development;

public class Script : Command<Script>, INamed
{
    public static string Name => "script";
    public override bool IsAllowedInPm => true;
    public override bool IsWhitelistOnly => true;
    public override bool IsHidden => true;

    private readonly IDependencyContainerService _dependencyContainerService;

    public Script(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public class Globals
    {
        public IContext Context;
        public IDependencyContainerService Container;
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
            Logger.Current.Error(exception, "An error occured while using script command.");
            context.Reply($"An error occured: {exception.Message}");
        }
    }
}