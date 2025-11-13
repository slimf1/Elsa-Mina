using ElsaMina.Core.Services.Images;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Services.RoomUserData;

public class RoomUserDataServiceTest
{
    private IRoomSpecificUserDataRepository _roomSpecificUserDataRepository;
    private IBadgeHoldingRepository _badgeHoldingRepository;
    private RoomUserDataService _service;

    [SetUp]
    public void SetUp()
    {
        _roomSpecificUserDataRepository = Substitute.For<IRoomSpecificUserDataRepository>();
        _badgeHoldingRepository = Substitute.For<IBadgeHoldingRepository>();
        _service = new RoomUserDataService(_roomSpecificUserDataRepository, _badgeHoldingRepository);
    }

    [Test]
    public async Task Test_GetUserData_ShouldReturnUserData_IfExists()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var existingUserData = new RoomSpecificUserData { Id = userId, RoomId = roomId };
        _roomSpecificUserDataRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(existingUserData);

        // Act
        var result = await _service.GetUserData(roomId, userId);

        // Assert
        Assert.That(result, Is.EqualTo(existingUserData));
    }

    [Test]
    public async Task Test_GetUserData_ShouldCreateUserData_IfNotExists()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        _roomSpecificUserDataRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns((RoomSpecificUserData)null);

        // Act
        var result = await _service.GetUserData(roomId, userId);

        // Assert
        await _roomSpecificUserDataRepository.Received().AddAsync(Arg.Is<RoomSpecificUserData>(u => u.Id == userId && u.RoomId == roomId));
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(userId));
            Assert.That(result.RoomId, Is.EqualTo(roomId));
        });
    }

    [Test]
    public async Task Test_InitializeJoinPhrases_ShouldPopulateJoinPhrasesDictionary()
    {
        // Arrange
        var userDataList = new List<RoomSpecificUserData>
        {
            new RoomSpecificUserData { Id = "user1", RoomId = "room1", JoinPhrase = "Hello" },
            new RoomSpecificUserData { Id = "user2", RoomId = "room2", JoinPhrase = "Welcome" }
        };
        _roomSpecificUserDataRepository.GetAllAsync().Returns(userDataList);

        // Act
        await _service.InitializeJoinPhrasesAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_service.JoinPhrases, Has.Count.EqualTo(2));
            Assert.That(_service.JoinPhrases[Tuple.Create("user1", "room1")], Is.EqualTo("Hello"));
            Assert.That(_service.JoinPhrases[Tuple.Create("user2", "room2")], Is.EqualTo("Welcome"));
        });
    }

    [Test]
    public async Task Test_GiveBadgeToUser_ShouldAddBadge()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var badgeId = "badge1";

        // Act
        await _service.GiveBadgeToUserAsync(roomId, userId, badgeId);

        // Assert
        await _badgeHoldingRepository.Received().AddAsync(Arg.Is<BadgeHolding>(b => b.BadgeId == badgeId && b.RoomId == roomId && b.UserId == userId));
    }

    [Test]
    public void Test_TakeBadgeFromUser_ShouldThrowArgumentException_WhenBadgeNotFound()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var badgeId = "badge1";
        _badgeHoldingRepository.GetByIdAsync(Arg.Any<Tuple<string, string, string>>()).Returns((BadgeHolding)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.TakeBadgeFromUserAsync(roomId, userId, badgeId));
    }

    [Test]
    public async Task Test_TakeBadgeFromUser_ShouldDeleteBadge_WhenFound()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var badgeId = "badge1";
        var badgeHolding = new BadgeHolding { BadgeId = badgeId, RoomId = roomId, UserId = userId };
        _badgeHoldingRepository.GetByIdAsync(Arg.Any<Tuple<string, string, string>>()).Returns(badgeHolding);

        // Act
        await _service.TakeBadgeFromUserAsync(roomId, userId, badgeId);

        // Assert
        await _badgeHoldingRepository.Received().DeleteAsync(badgeHolding);
    }

    [Test]
    public void Test_SetUserTitle_ShouldThrowArgumentException_WhenTitleTooLong()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var title = new string('a', 451);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.SetUserTitleAsync(roomId, userId, title));
    }

    [Test]
    public async Task Test_SetUserTitle_ShouldUpdateUserDataTitle_WhenValid()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var title = "Valid Title";
        var userData = new RoomSpecificUserData { Id = userId, RoomId = roomId };
        _roomSpecificUserDataRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(userData);

        // Act
        await _service.SetUserTitleAsync(roomId, userId, title);

        // Assert
        Assert.That(userData.Title, Is.EqualTo(title));
        await _roomSpecificUserDataRepository.Received().SaveChangesAsync();
    }

    [Test]
    public void Test_SetUserAvatar_ShouldThrowArgumentException_WhenInvalidUrl()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var invalidAvatar = "invalid_url";

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.SetUserAvatarAsync(roomId, userId, invalidAvatar));
    }

    [Test]
    public async Task Test_SetUserAvatar_ShouldUpdateUserDataAvatar_WhenValid()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var avatar = "https://valid.url/image.jpg";
        var userData = new RoomSpecificUserData { Id = userId, RoomId = roomId };
        _roomSpecificUserDataRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(userData);

        // Act
        await _service.SetUserAvatarAsync(roomId, userId, avatar);

        // Assert
        Assert.That(userData.Avatar, Is.EqualTo(avatar));
        await _roomSpecificUserDataRepository.Received().SaveChangesAsync();
    }

    [Test]
    public void Test_SetUserJoinPhrase_ShouldThrowArgumentException_WhenJoinPhraseTooLong()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var joinPhrase = new string('a', 301);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.SetUserJoinPhraseAsync(roomId, userId, joinPhrase));
    }

    [Test]
    public async Task Test_SetUserJoinPhrase_ShouldUpdateJoinPhrase_WhenValid()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var joinPhrase = "Welcome!";
        var userData = new RoomSpecificUserData { Id = userId, RoomId = roomId };
        _roomSpecificUserDataRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(userData);

        // Act
        await _service.SetUserJoinPhraseAsync(roomId, userId, joinPhrase);

        // Assert
        Assert.That(userData.JoinPhrase, Is.EqualTo(joinPhrase));
        await _roomSpecificUserDataRepository.Received().SaveChangesAsync();
        Assert.That(_service.JoinPhrases[Tuple.Create(userId, roomId)], Is.EqualTo(joinPhrase));
    }
}