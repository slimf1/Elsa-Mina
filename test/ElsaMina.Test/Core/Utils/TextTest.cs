using ElsaMina.Core.Utils;

namespace ElsaMina.Test.Core.Utils;

public class TextTest
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
    
    [Test]
    [TestCase("n_n\n", ExpectedResult = "n_n")]
    public string Test_RemoveNewLines_ShouldRemoveNewLines(string input)
    {
        // Act
        var result = input.RemoveNewlines();

        // Assert
        Assert.That(result, Is.Not.Null);
        return result;
    }
    
    [Test]
    [TestCase("<a> <b>    </b></a>", ExpectedResult = "<a><b></b></a>")]
    [TestCase(" <test>     <a>   </a>   <myTag> lol </myTag> </test>", ExpectedResult = "<test><a></a><myTag>lol</myTag></test>")]
    public string Test_RemoveWhitespacesBetweenTags_ShouldRemoveWhitespaces(string input)
    {
        // Act
        var result = input.RemoveWhitespacesBetweenTags();

        // Assert
        Assert.That(result, Is.Not.Null);
        return result;
    }
    
    [Test]
    [TestCase("", ExpectedResult = "")]
    [TestCase("lol", ExpectedResult = "Lol")]
    [TestCase("test lol", ExpectedResult = "Test lol")]
    public string Test_Capitalize_ShouldCapitalizeFirstWord(string input)
    {
        // Act
        var result = input.Capitalize();

        // Assert
        Assert.That(result, Is.Not.Null);
        return result;
    }
    
    [Test]
    [TestCase("", "", ExpectedResult = 0)]
    [TestCase("a", "a", ExpectedResult = 0)]
    [TestCase("aa", "ab", ExpectedResult = 1)]
    [TestCase("xDlol", "xMlal", ExpectedResult = 2)]
    [TestCase("test string lol", "tst strng loool", ExpectedResult = 4)]
    public int Test_LevenshteinDistance_ShouldCalculateDistance(string s1, string s2)
    {
        // Act
        var result = Text.LevenshteinDistance(s1, s2);

        // Assert
        return result;
    }
    
    [Test]
    [TestCase("lol mdrrrrrr", 5, ExpectedResult = "lol...")]
    [TestCase("lol mdr xD", 8, ExpectedResult = "lol mdr...")]
    [TestCase("lol mdr xD", 10, ExpectedResult = "lol mdr xD")]
    public string Test_Shorten_ShouldShortenWithoutCuttingWords(string text, int maxLength)
    {
        // Act
        var result = text.Shorten(maxLength);

        // Assert
        return result;
    }
}