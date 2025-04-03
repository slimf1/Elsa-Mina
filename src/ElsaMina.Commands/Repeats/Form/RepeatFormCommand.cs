using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Repeats;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Repeats.Form;

[NamedCommand("repeat", Aliases = ["create-repeat"])]
public class RepeatFormCommand : Command
{
    private readonly IRepeatsManager _repeatsManager;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfiguration _configuration;

    public RepeatFormCommand(IRepeatsManager repeatsManager,
        ITemplatesManager templatesManager,
        IConfiguration configuration)
    {
        _repeatsManager = repeatsManager;
        _templatesManager = templatesManager;
        _configuration = configuration;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override string HelpMessageKey => "aboutrepeat_helpmessage";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var template = await _templatesManager.GetTemplateAsync("Repeats/Form/RepeatForm", new RepeatFormViewModel
        {
            Culture = context.Culture,
            Command = $"/w {_configuration.Name},{_configuration.Trigger}startrepeat {{message}}, {{delay}}"
        });

        context.SendHtmlTo(context.Sender.UserId, template.RemoveNewlines());
    }
}