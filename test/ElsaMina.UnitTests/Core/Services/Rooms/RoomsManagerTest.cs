using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.DataAccess;
using ModernBOT.Api.Data.Models;
using ModernBOT.Api.Data.Repositories;
using ModernBOT.Api.Helpers;
using ModernBOT.Api.Helpers.Services;
using ModernBOT.Api.Helpers.Model;
using ModernBOT.Api.Parameters.Interfaces;
using ModernBOT.Api.Parameters.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;

namespace ModernBOT.Api.Helpers
{
    [TestFixture]
    public class RoomsManagerTest
    {
        private IConfiguration _configuration;
        private IRoomInfoRepository _roomInfoRepository;
        private IParametersFactory _parametersFactory;
        private IBotDbContextFactory _dbContextFactory;
        private IBotDbContext _dbContext;
        private IRoomUserDataService _roomUserDataService;

        private RoomsManager _roomsManager;

        [SetUp]
        public void SetUp()
        {
            _configuration = Substitute.For<IConfiguration>();
            _roomInfoRepository = Substitute.For<IRoomInfoRepository>();
            _parametersFactory = Substitute.For<IParametersFactory>();
            _dbContextFactory = Substitute.For<IBotDbContextFactory>();
            _dbContext = Substitute.For<IBotDbContext>();
            _roomUserDataService = Substitute.For<IRoomUserDataService>();

            // Mock PlayTime Update Interval
            _configuration.PlayTimeUpdatesInterval.Returns(TimeSpan.FromDays(1));

            // Fake parameters dictionary
            var mockParameter = Substitute.For<IParameter>();
            _parametersFactory.GetParameters().Returns(
                new Dictionary<string, IParameter>()
                {
                    { ParametersConstants.LOCALE, mockParameter }
                });

            // Setup DB Context creation
            _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(_dbContext));

            // Mock DbSet<Room>
            var roomsDbSet = CreateMockDbSet<Room>();
            _dbContext.RoomInfo.Returns(roomsDbSet);

            // Create the RoomsManager instance
            _roomsManager = new RoomsManager(
                _configuration,
                _parametersFactory,
                _dbContextFactory,
                _roomUserDataService
            );
        }

        // Utility to mock DbSet
        private static DbSet<T> CreateMockDbSet<T>() where T : class
        {
            var queryable = new List<T>().AsQueryable();

            var mockSet = Substitute.For<DbSet<T>, IQueryable<T>>();

            ((IQueryable<T>)mockSet).Provider.Returns(queryable.Provider);
            ((IQueryable<T>)mockSet).Expression.Returns(queryable.Expression);
            ((IQueryable<T>)mockSet).ElementType.Returns(queryable.ElementType);
            ((IQueryable<T>)mockSet).GetEnumerator().Returns(queryable.GetEnumerator());

            return mockSet;
        }

        // ---------------------------------------------------------------------
        //                          TESTS
        // ---------------------------------------------------------------------

        [Test]
        public async Task Test_InitializeRoomAsync_ShouldAddRoom_WhenRoomDoesNotExist()
        {
            // Arrange
            var roomName = "myRoom";

            _roomInfoRepository.GetWithDetailsAsync(
                Arg.Any<Expression<Func<Room, bool>>>()
            ).Returns(Task.FromResult<Room>(null));

            Room capturedRoom = null;
            await _roomInfoRepository.AddAsync(Arg.Do<Room>(r => capturedRoom = r));

            // Act
            await _roomsManager.InitializeRoomAsync(roomName);

            // Assert
            Assert.NotNull(capturedRoom);
            Assert.AreEqual(roomName, capturedRoom.Name);

            await _roomInfoRepository.Received(1).AddAsync(Arg.Any<Room>());
            await _roomInfoRepository.Received(1).SaveChangesAsync();
        }

}
