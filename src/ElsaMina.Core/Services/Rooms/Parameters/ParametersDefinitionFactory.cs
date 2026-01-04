using System.Globalization;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Services.Rooms.Parameters;

public class ParametersDefinitionFactory : IParametersDefinitionFactory
{
    private readonly IConfiguration _configuration;
    private readonly IResourcesService _resourcesService;
    
    public ParametersDefinitionFactory(IConfiguration configuration,
        IResourcesService resourcesService)
    {
        _configuration = configuration;
        _resourcesService = resourcesService;
    }

    public IReadOnlyDictionary<Parameter, IParameterDefinition> GetParametersDefinitions() =>
        new Dictionary<Parameter, IParameterDefinition>
        {
            [Parameter.Locale] = new ParameterDefinition
            {
                Identifier = "locale",
                NameKey = "parameter_name_locale",
                DescriptionKey = "parameter_description_locale",
                Type = RoomBotConfigurationType.Enumeration,
                DefaultValue = _configuration.DefaultLocaleCode,
                PossibleValues = _resourcesService.SupportedLocales.Select(culture => new EnumerationValue
                {
                    DisplayedValue = culture.NativeName.Capitalize(),
                    InternalValue = culture.Name
                }),
                OnUpdateAction = (room, newValue) => room.Culture = new CultureInfo(newValue)
            },
            [Parameter.HasCommandAutoCorrect] = new ParameterDefinition
            {
                Identifier = "autocorr",
                NameKey = "parameter_name_has_command_auto_correct",
                DescriptionKey = "parameter_description_has_command_auto_correct",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            },
            [Parameter.ShowErrorMessages] = new ParameterDefinition
            {
                Identifier = "error",
                NameKey = "parameter_name_is_showing_error_messages",
                DescriptionKey = "parameter_description_is_showing_error_messages",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            },
            [Parameter.ShowTeamLinksPreview] = new ParameterDefinition
            {
                Identifier = "teams",
                NameKey = "parameter_name_is_showing_team_links_preview",
                DescriptionKey = "parameter_description_is_showing_team_links_preview",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            },
            [Parameter.ShowReplaysPreview] = new ParameterDefinition
            {
                Identifier = "replays",
                NameKey = "parameter_name_is_showing_replays_preview",
                DescriptionKey = "parameter_description_is_showing_replays_preview",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            }
        };
}