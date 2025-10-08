using System.Text.RegularExpressions;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Images;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;
using NCalc;

namespace ElsaMina.Core.Services.AddedCommands;

public class AddedCommandsManager : IAddedCommandsManager
{
    private const int MAX_HEIGHT = 300;
    private const int MAX_WIDTH = 400;

    private static readonly Regex EXPRESSION_IDENTIFIER =
        new("{([^}]+)}", RegexOptions.Compiled, Constants.REGEX_MATCH_TIMEOUT);

    private readonly IAddedCommandRepository _addedCommandRepository;
    private readonly IImageService _imageService;
    private readonly IRandomService _randomService;

    private readonly Dictionary<string, Func<object[], object>> _predefinedFunctions = new();
    private readonly Dictionary<string, Func<object>> _predefinedIdentifiers = new();

    public AddedCommandsManager(IAddedCommandRepository addedCommandRepository,
        IImageService imageService,
        IRandomService randomService)
    {
        _addedCommandRepository = addedCommandRepository;
        _imageService = imageService;
        _randomService = randomService;
    }

    public async Task TryExecuteAddedCommand(string commandName, IContext context)
    {
        var command = await _addedCommandRepository.GetByIdAsync(Tuple.Create(commandName, context.RoomId));
        if (command == null)
        {
            return;
        }

        var content = command.Content;
        if (content.IsValidImageLink())
        {
            await DisplayRemoteImage(context, content);
            return;
        }

        content = EvaluateContent(content, context);
        context.Reply(content, rankAware: true);
    }

    private async Task DisplayRemoteImage(IContext context, string content)
    {
        var (width, height) = await _imageService.GetRemoteImageDimensions(content);
        (width, height) = _imageService.ResizeWithSameAspectRatio(width, height, MAX_WIDTH, MAX_HEIGHT);
        context.SendHtmlIn($"""<img src="{content}" width="{width}" height="{height}" />""", rankAware: true);
    }

    #region Content Expression Parsing

    internal string EvaluateContent(string content, IContext context)
    {
        InitializeDefinitions(context);

        var matches = EXPRESSION_IDENTIFIER.Matches(content);
        foreach (Match match in matches)
        {
            var rawExpression = match.Groups[1].Value;
            var result = EvaluateExpression(new Expression(rawExpression));
            content = content.Replace(match.Value, result.ToString());
        }

        return content;
    }

    private object EvaluateExpression(Expression expression)
    {
        foreach (var identifier in _predefinedIdentifiers)
        {
            expression.Parameters[identifier.Key] = identifier.Value.Invoke();
        }

        foreach (var function in _predefinedFunctions)
        {
            expression.EvaluateFunction += (name, args) =>
            {
                if (name == function.Key)
                {
                    args.Result = function.Value.DynamicInvoke((object)args.EvaluateParameters());
                }
            };
        }

        return expression.Evaluate();
    }

    private void InitializeDefinitions(IContext context)
    {
        // Pas très opti en effet
        _predefinedFunctions.Clear();
        _predefinedIdentifiers.Clear();

        _predefinedFunctions.Add("choice", args => _randomService.RandomElement(args));
        _predefinedFunctions.Add("dice",
            args => _randomService.NextInt(Convert.ToInt32(args[0].ToString()), Convert.ToInt32(args[1]) + 1));
        _predefinedFunctions.Add("repeat", args => string.Concat(Enumerable.Repeat(args[0], Convert.ToInt32(args[1]))));
        _predefinedFunctions.Add("optional",
            args => _randomService.NextDouble() < Convert.ToDouble(args[1]) ? args[0] : string.Empty);
        _predefinedFunctions.Add("cos", args => Math.Cos(Convert.ToDouble(args[0])));
        _predefinedFunctions.Add("sin", args => Math.Sin(Convert.ToDouble(args[0])));
        _predefinedFunctions.Add("tan", args => Math.Tan(Convert.ToDouble(args[0])));

        _predefinedIdentifiers.Add("command", () => context.Command);
        _predefinedIdentifiers.Add("author", () => context.Sender.Name);
        _predefinedIdentifiers.Add("room", () => context.Room.Name);
        _predefinedIdentifiers.Add("randomMember",
            () => _randomService.RandomElement(context.Room.Users.Select(userKvp => userKvp.Value.Name)));
        _predefinedIdentifiers.Add("args", () => context.Target);
        _predefinedIdentifiers.Add("e", () => Math.E);
        _predefinedIdentifiers.Add("pi", () => Math.PI);
    }

    #endregion
}