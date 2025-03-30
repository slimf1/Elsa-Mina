using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.System;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public class CheckConnectionHandler : Handler
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IClient _client;
    private readonly ISystemService _systemService;

    public CheckConnectionHandler(IConfigurationManager configurationManager, IClient client,
        ISystemService systemService)
    {
        _configurationManager = configurationManager;
        _client = client;
        _systemService = systemService;
    }

    public override async Task HandleReceivedMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default)
    {
        if (parts.Length >= 2 && parts[1] == "updateuser")
        {
            var name = parts[2][1..];
            if (name.Contains("Guest")) // We need to be logged in to join some rooms
            {
                return;
            }

            if (name.Contains('@'))
            {
                name = name[^2..];
            }

            Log.Information("Connected as : {0}", name);

            foreach (var roomIdToJoin in _configurationManager.Configuration.Rooms)
            {
                if (_configurationManager.Configuration.RoomBlacklist.Contains(roomIdToJoin))
                {
                    continue;
                }

                _client.Send($"|/join {roomIdToJoin}");
                await _systemService.SleepAsync(TimeSpan.FromMilliseconds(250));
            }
        }
    }
}