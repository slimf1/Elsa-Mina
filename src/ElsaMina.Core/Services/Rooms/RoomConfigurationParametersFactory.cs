using System.Globalization;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Services.Rooms;

public class RoomConfigurationParametersFactory : IRoomConfigurationParametersFactory
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;
    
    public RoomConfigurationParametersFactory(IConfigurationManager configurationManager,
        IResourcesService resourcesService)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
    }

    public IReadOnlyDictionary<string, RoomBotConfigurationParameter> GetParameters() =>
        new Dictionary<string, RoomBotConfigurationParameter>()
        {
            [RoomParametersConstants.LOCALE] = new()
            {
                Identifier = RoomParametersConstants.LOCALE,
                NameKey = "parameter_name_locale",
                DescriptionKey = "parameter_description_locale",
                Type = RoomBotConfigurationType.String,
                DefaultValue = _configurationManager.Configuration.DefaultLocaleCode,
                PossibleValues = _resourcesService.SupportedLocales.Select(culture => new EnumerationValue
                {
                    DisplayedValue = culture.DisplayName,
                    InternalValue = culture.Name
                }),
                OnUpdateAction = (room, newValue) =>
                {
                    try
                    {
                        room.Culture = new CultureInfo(newValue);
                    }
                    catch (CultureNotFoundException)
                    {
                        // Do nothing
                    }
                }
            },
            [RoomParametersConstants.HAS_COMMAND_AUTO_CORRECT] = new()
            {
                Identifier = RoomParametersConstants.HAS_COMMAND_AUTO_CORRECT,
                NameKey = "parameter_name_has_command_auto_correct",
                DescriptionKey = "parameter_description_has_command_auto_correct",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            },
            [RoomParametersConstants.IS_SHOWING_ERROR_MESSAGES] = new()
            {
                Identifier = RoomParametersConstants.IS_SHOWING_ERROR_MESSAGES,
                NameKey = "parameter_name_is_showing_team_links_preview",
                DescriptionKey = "parameter_description_is_showing_error_messages",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            },
            [RoomParametersConstants.IS_SHOWING_TEAM_LINKS_PREVIEW] = new()
            {
                Identifier = RoomParametersConstants.IS_SHOWING_TEAM_LINKS_PREVIEW,
                NameKey = "parameter_name_is_showing_team_links_preview",
                DescriptionKey = "parameter_description_is_showing_team_links_preview",
                Type = RoomBotConfigurationType.Boolean,
                DefaultValue = true.ToString()
            }
        };
}