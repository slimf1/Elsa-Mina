using System.Globalization;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Services.Rooms.Parameters;

public class ParametersFactory : IParametersFactory
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;
    
    public ParametersFactory(IConfigurationManager configurationManager,
        IResourcesService resourcesService)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
    }

    public IReadOnlyDictionary<string, IParameter> GetParameters() =>
        new Dictionary<string, IParameter>
        {
            [ParametersConstants.LOCALE] = new Parameter
            {
                Identifier = ParametersConstants.LOCALE,
                NameKey = "parameter_name_locale",
                DescriptionKey = "parameter_description_locale",
                Type = RoomBotConfigurationType.Enumeration,
                DefaultValue = _configurationManager.Configuration.DefaultLocaleCode,
                PossibleValues = _resourcesService.SupportedLocales.Select(culture => new EnumerationValue
                {
                    DisplayedValue = culture.NativeName.Capitalize(),
                    InternalValue = culture.Name
                }),
                OnUpdateAction = (room, newValue) => room.Culture = new CultureInfo(newValue)
            },
            [ParametersConstants.HAS_COMMAND_AUTO_CORRECT] = new Parameter
            {
                Identifier = ParametersConstants.HAS_COMMAND_AUTO_CORRECT,
                NameKey = "parameter_name_has_command_auto_correct",
                DescriptionKey = "parameter_description_has_command_auto_correct",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            },
            [ParametersConstants.IS_SHOWING_ERROR_MESSAGES] = new Parameter
            {
                Identifier = ParametersConstants.IS_SHOWING_ERROR_MESSAGES,
                NameKey = "parameter_name_is_showing_error_messages",
                DescriptionKey = "parameter_description_is_showing_error_messages",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            },
            [ParametersConstants.IS_SHOWING_TEAM_LINKS_PREVIEW] = new Parameter
            {
                Identifier = ParametersConstants.IS_SHOWING_TEAM_LINKS_PREVIEW,
                NameKey = "parameter_name_is_showing_team_links_preview",
                DescriptionKey = "parameter_description_is_showing_team_links_preview",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            },
            [ParametersConstants.IS_SHOWING_REPLAYS_PREVIEW] = new Parameter
            {
                Identifier = ParametersConstants.IS_SHOWING_REPLAYS_PREVIEW,
                NameKey = "parameter_name_is_showing_replays_preview",
                DescriptionKey = "parameter_description_is_showing_replays_preview",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            }
        };
}