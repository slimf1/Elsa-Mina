using ElsaMina.Core.Utils;

namespace ElsaMina.UnitTests.Core.Utils;

public class FileSystemTest
{
    // TODO : make the bot cross-platform as this doesn't work on Windows

    [Test]
    public void Test_MakeRelativePath_ShouldReturnRelativePath()
    {
        // Arrange
        const string absolutePath = "/Library/Stuff/Lol/Test/Stuff.png";

        // Act
        var result = FileSystem.MakeRelativePath(absolutePath, referencePath: "/Library/Stuff/Lol");

        // Assert
        Assert.That(result, Is.EqualTo("Lol/Test/Stuff.png"));
    }
}