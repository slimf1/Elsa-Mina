using ElsaMina.Core.Services.Images;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;

namespace ElsaMina.Test.Core.Services.RoomUserData;

public class RoomUserDataServiceTest
{
    private IRoomSpecificUserDataRepository _roomSpecificUserDataRepository;
    private IBadgeHoldingRepository _badgeHoldingRepository;
    private RoomUserDataService _service;
    private IImageService _imageService;

    [SetUp]
    public void SetUp()
    {
        _roomSpecificUserDataRepository = Substitute.For<IRoomSpecificUserDataRepository>();
        _badgeHoldingRepository = Substitute.For<IBadgeHoldingRepository>();
        _imageService = Substitute.For<IImageService>();
        _service = new RoomUserDataService(_roomSpecificUserDataRepository, _badgeHoldingRepository, _imageService);
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
        await _service.InitializeJoinPhrases();

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
        await _service.GiveBadgeToUser(roomId, userId, badgeId);

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
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.TakeBadgeFromUser(roomId, userId, badgeId));
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
        await _service.TakeBadgeFromUser(roomId, userId, badgeId);

        // Assert
        await _badgeHoldingRepository.Received().DeleteByIdAsync(Arg.Any<Tuple<string, string, string>>());
    }

    [Test]
    public void Test_SetUserTitle_ShouldThrowArgumentException_WhenTitleTooLong()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var title = new string('a', 451);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.SetUserTitle(roomId, userId, title));
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
        await _service.SetUserTitle(roomId, userId, title);

        // Assert
        Assert.That(userData.Title, Is.EqualTo(title));
        await _roomSpecificUserDataRepository.Received().UpdateAsync(userData);
    }

    [Test]
    public void Test_SetUserAvatar_ShouldThrowArgumentException_WhenInvalidUrl()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var invalidAvatar = "invalid_url";
        _imageService.IsImageLink(invalidAvatar).Returns(false);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.SetUserAvatar(roomId, userId, invalidAvatar));
    }

    [Test]
    public async Task Test_SetUserAvatar_ShouldUpdateUserDataAvatar_WhenValid()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var avatar = "https://valid.url/image.jpg";
        _imageService.IsImageLink(avatar).Returns(true);
        var userData = new RoomSpecificUserData { Id = userId, RoomId = roomId };
        _roomSpecificUserDataRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(userData);

        // Act
        await _service.SetUserAvatar(roomId, userId, avatar);

        // Assert
        Assert.That(userData.Avatar, Is.EqualTo(avatar));
        await _roomSpecificUserDataRepository.Received().UpdateAsync(userData);
    }

    [Test]
    public void Test_SetUserJoinPhrase_ShouldThrowArgumentException_WhenJoinPhraseTooLong()
    {
        // Arrange
        var roomId = "room1";
        var userId = "user1";
        var joinPhrase = new string('a', 301);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.SetUserJoinPhrase(roomId, userId, joinPhrase));
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
        await _service.SetUserJoinPhrase(roomId, userId, joinPhrase);

        // Assert
        Assert.That(userData.JoinPhrase, Is.EqualTo(joinPhrase));
        await _roomSpecificUserDataRepository.Received().UpdateAsync(userData);
        Assert.That(_service.JoinPhrases[Tuple.Create(userId, roomId)], Is.EqualTo(joinPhrase));
    }
}