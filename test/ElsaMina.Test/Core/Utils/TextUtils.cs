using ElsaMina.Core.Utils;

namespace ElsaMina.Test.Core.Utils;

public class TextUtilsTest
{
    [Test]
    [TestCase("Test! 123", ExpectedResult = "test123")]
    [TestCase("This is a test.", ExpectedResult = "thisisatest")]
    [TestCase("Hello, World!", ExpectedResult = "helloworld")]
    public string Test_ToLowerAlphaNum_ShouldReformatString(string input)
    {
        // Act
        var result = input.ToLowerAlphaNum();

        // Assert
        Assert.That(result, Is.Not.Null);
        return result;
    }
}