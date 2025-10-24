using ElsaMina.Core.Utils;

namespace ElsaMina.UnitTests.Core.Utils;

public class FileSystemTest
{
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