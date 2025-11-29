using System.Globalization;
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

namespace ElsaMina.IntegrationTests.Core.Services.Rooms;

public class RoomTest
{
    private BotDbContext _context;
    private RoomsManager _roomsManager;
    private IUserPlayTimeRepository _userPlayTimeRepository;
    
    [SetUp]
    public async Task SetUp()
    {
        var opts = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase("BotDbIntegrationTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _context = new BotDbContext(opts);
        _userPlayTimeRepository = Substitute.For<IUserPlayTimeRepository>();
        var configuration = Substitute.For<IConfiguration>();
        configuration.PlayTimeUpdatesInterval.Returns(TimeSpan.MaxValue);
        configuration.DefaultLocaleCode.Returns("fr-FR");
        var resourcesService = Substitute.For<IResourcesService>();
        resourcesService.SupportedLocales.Returns([new CultureInfo("fr-FR")]);
        var parametersFactory = new ParametersDefinitionFactory(configuration,
            resourcesService);
        var roomParametersRepository = new RoomInfoRepository(_context);
        var roomBotParameterValueRepository = new RoomBotParameterValueRepository(_context);
        _roomsManager = new RoomsManager(configuration, parametersFactory,
            roomParametersRepository, roomBotParameterValueRepository, _userPlayTimeRepository);
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
        _roomsManager.Dispose();
        _context.Dispose();
        _userPlayTimeRepository.Dispose();
    }

    [Test]
    public async Task Test_RoomConfiguration_ShouldUpdateDatabase()
    {
        // Get default value
        Assert.That(_roomsManager
                .GetRoomParameter("franais", ParametersConstants.LOCALE),
            Is.EqualTo("fr-FR"));
        
        // Modify value
        var result = await _roomsManager
            .SetRoomParameter("franais", ParametersConstants.LOCALE, "en-US");
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_roomsManager
                    .GetRoomParameter("franais", ParametersConstants.LOCALE),
                Is.EqualTo("en-US"));
        });
        await _roomsManager.SetRoomParameter("franais",
            ParametersConstants.HAS_COMMAND_AUTO_CORRECT, false.ToString());
        Assert.That(_roomsManager
                .GetRoomParameter("franais", ParametersConstants.HAS_COMMAND_AUTO_CORRECT).ToBoolean(),
            Is.False);
    }
}