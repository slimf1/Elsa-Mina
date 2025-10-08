using ElsaMina.Core.Utils;

namespace ElsaMina.UnitTests.Core.Utils;

public class StringExtensionsTest
{
    [Test]
    [TestCase(null, ExpectedResult = false)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("e", ExpectedResult = false)]
    [TestCase("https://youtube.com", ExpectedResult = false)]
    [TestCase("https://example/image.png", ExpectedResult = true)]
    [TestCase("https://example/image.gif", ExpectedResult = true)]
    [TestCase("https://example/image.jpg", ExpectedResult = true)]
    public bool Test_IsValidImageLink_ShouldReturnTrue_WhenLinkIsAnImage(string link)
    {
        // Act & Assert
        return link.IsValidImageLink();
    }
}