using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.CustomColors;
using ElsaMina.Core.Services.Dex;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Core.Services.Start;

public class StartManager : IStartManager
{
    private readonly ITemplatesManager _templatesManager;
    private readonly ICommandExecutor _commandExecutor;
    private readonly ICustomColorsManager _customColorsManager;
    private readonly IRoomUserDataService _roomUserDataService;
    private readonly IDexManager _dexManager;

    public StartManager(ITemplatesManager templatesManager,
        ICommandExecutor commandExecutor,
        ICustomColorsManager customColorsManager,
        IRoomUserDataService roomUserDataService,
        IDexManager dexManager)
    {
        _templatesManager = templatesManager;
        _commandExecutor = commandExecutor;
        _customColorsManager = customColorsManager;
        _roomUserDataService = roomUserDataService;
        _dexManager = dexManager;
    }

    public async Task OnStart()
    {
        // Static data / referentials
        await _templatesManager.CompileTemplates();
        await _customColorsManager.FetchCustomColors();
        await _roomUserDataService.InitializeJoinPhrases();
        await _dexManager.LoadDex();
    }
}