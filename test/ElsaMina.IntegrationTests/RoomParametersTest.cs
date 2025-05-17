using System.Globalization;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;

namespace ElsaMina.IntegrationTests;

public class RoomParametersTest
{
    private BotDbContext _context;
    private RoomsManager _roomsManager;
    private IUserPlayTimeRepository _userPlayTimeRepository;
    private IClockService _clockService;
    
    [SetUp]
    public async Task SetUp()
    {
        var opts = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase("BotDbIntegrationTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _context = new BotDbContext(opts);
        _userPlayTimeRepository = Substitute.For<IUserPlayTimeRepository>();
        _clockService = Substitute.For<IClockService>();
        var configurationManager = Substitute.For<IConfigurationManager>();
        configurationManager.Configuration.Returns(new Configuration
        {
            DefaultLocaleCode = "fr-FR"
        });
        var resourcesService = Substitute.For<IResourcesService>();
        resourcesService.SupportedLocales.Returns([new CultureInfo("fr-FR")]);
        var roomConfigurationParametersFactory = new RoomConfigurationParametersFactory(configurationManager,
            resourcesService);
        var roomParametersRepository = new RoomParametersRepository(_context);
        var roomBotParameterValueRepository = new RoomBotParameterValueRepository(_context);
        _roomsManager = new RoomsManager(configurationManager, roomConfigurationParametersFactory,
            roomParametersRepository, roomBotParameterValueRepository, _userPlayTimeRepository, _clockService);
        string[] lines =
        [
            ">franais",
            "|init|chat",
            "|title|Français",
            "|users|4,&Teclis,!Lionyx,@Earth, Mec"
        ];
        await _roomsManager.InitializeRoomAsync("franais", lines);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _userPlayTimeRepository.Dispose();
    }

    [Test]
    public async Task Test_RoomConfiguration_ShouldUpdateDatabase()
    {
        // Get default value
        Assert.That(_roomsManager
                .GetRoomConfigurationParameter("franais", RoomParametersConstants.LOCALE),
            Is.EqualTo("fr-FR"));
        
        // Modify value
        var result = await _roomsManager
            .SetRoomConfigurationParameter("franais", RoomParametersConstants.LOCALE, "en-US");
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_roomsManager
                    .GetRoomConfigurationParameter("franais", RoomParametersConstants.LOCALE),
                Is.EqualTo("en-US"));
        });
        await _roomsManager.SetRoomConfigurationParameter("franais",
            RoomParametersConstants.HAS_COMMAND_AUTO_CORRECT, false.ToString());
        Assert.That(_roomsManager
                .GetRoomConfigurationParameter("franais", RoomParametersConstants.HAS_COMMAND_AUTO_CORRECT).ToBoolean(),
            Is.False);
    }
}