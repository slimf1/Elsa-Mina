using ElsaMina.Core.Services.CustomColors;
using ElsaMina.Core.Services.Dex;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Core.Services.Start;

public class StartManager : IStartManager
{
    private readonly ITemplatesManager _templatesManager;
    private readonly ICustomColorsManager _customColorsManager;
    private readonly IRoomUserDataService _roomUserDataService;
    private readonly IDexManager _dexManager;

    public StartManager(ITemplatesManager templatesManager,
        ICustomColorsManager customColorsManager,
        IRoomUserDataService roomUserDataService,
        IDexManager dexManager)
    {
        _templatesManager = templatesManager;
        _customColorsManager = customColorsManager;
        _roomUserDataService = roomUserDataService;
        _dexManager = dexManager;
    }

    public async Task OnStart()
    {
        // Static data / referentials
        await Task.WhenAll(
            _templatesManager.CompileTemplatesAsync(),
            _customColorsManager.FetchCustomColorsAsync(),
            _roomUserDataService.InitializeJoinPhrasesAsync(),
            _dexManager.LoadDexAsync()
        );
    }
}