using ElsaMina.Commands.Profile;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.UnitTests.Commands.Profile;

public class ProfileServiceTest
{
    [Test]
    public void Test_GetAvatar_ShouldReturnDefaultAvatar_WhenNoStoredOrShowdownAvatar()
    {
        var avatar = ProfileService.GetAvatar(null, null);

        Assert.That(avatar, Is.EqualTo("https://play.pokemonshowdown.com/sprites/trainers/unknown.png"));
    }

    [Test]
    public void Test_GetAvatar_ShouldReturnCustomStoredAvatar_WhenPresent()
    {
        var stored = new RoomUser { Avatar = "https://custom/avatar.png" };

        var avatar = ProfileService.GetAvatar(stored, null);

        Assert.That(avatar, Is.EqualTo("https://custom/avatar.png"));
    }

    [Test]
    public void Test_GetAvatar_ShouldReturnCustomUrl_WhenAvatarStartsWithHash()
    {
        var details = new UserDetailsDto { Avatar = "#123" };

        var avatar = ProfileService.GetAvatar(null, details);

        Assert.That(avatar.Contains("trainers-custom/123.png"), Is.True);
    }
}
