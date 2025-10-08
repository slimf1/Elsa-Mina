using ElsaMina.Core.Services.CustomColors;
using ElsaMina.Core.Services.Dex;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Core.Services.Start;

public class StartManager : IStartManager
{
    private readonly ITemplatesManager _templatesManager;
    private readonly ICustomColorsManager _customColorsManager;
    private readonly IRoomUserDataService _roomUserDataService;
    private readonly IDexManager _dexManager;
    private readonly IRoomsManager _roomsManager;

    public StartManager(ITemplatesManager templatesManager,
        ICustomColorsManager customColorsManager,
        IRoomUserDataService roomUserDataService,
        IDexManager dexManager, IRoomsManager roomsManager)
    {
        _templatesManager = templatesManager;
        _customColorsManager = customColorsManager;
        _roomUserDataService = roomUserDataService;
        _dexManager = dexManager;
        _roomsManager = roomsManager;
    }

    public async Task LoadStaticDataAsync(CancellationToken cancellationToken = default)
    {
        _roomsManager.Initialize();
        await Task.WhenAll(
            _templatesManager.CompileTemplatesAsync(),
            _customColorsManager.FetchCustomColorsAsync(cancellationToken),
            _roomUserDataService.InitializeJoinPhrasesAsync(cancellationToken),
            _dexManager.LoadDexAsync(cancellationToken)
        );
    }
}