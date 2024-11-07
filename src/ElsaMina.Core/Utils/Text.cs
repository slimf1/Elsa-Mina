using System.Text;
using System.Text.RegularExpressions;

namespace ElsaMina.Core.Utils;

public static partial class Text
{
    public static string ToLowerAlphaNum(this string text)
    {
        return ToLowerAlphaNumFilterRegex().Replace(text.ToLower(), "");
    }

    public static string RemoveNewlines(this string text)
    {
        return text.Replace("\n", string.Empty);
    }

    public static string Capitalize(this string text)
    {
        return text[0].ToString().ToUpper() + text[1..];
    }

    public static bool ToBoolean(this string text)
    {
        return text.Trim().ToLower() is "true" or "y" or "t" or "1" or "on";
    }
    
    /// <remarks>
    /// Credit : https://gist.github.com/Davidblkx/e12ab0bb2aff7fd8072632b396538560
    /// </remarks>
    public static int LevenshteinDistance(string source1, string source2)
    {
        var source1Length = source1.Length;
        var source2Length = source2.Length;

        var matrix = new int[source1Length + 1, source2Length + 1];

        if (source1Length == 0)
        {
            return source2Length;
        }

        if (source2Length == 0)
        {
            return source1Length;
        }

        for (var i = 0; i <= source1Length; matrix[i, 0] = i++)
        {
            // Do nothing
        }

        for (var j = 0; j <= source2Length; matrix[0, j] = j++)
        {
            // Do nothing
        }

        for (var i = 1; i <= source1Length; i++)
        {
            for (var j = 1; j <= source2Length; j++)
            {
                var cost = source2[j - 1] == source1[i - 1] ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[source1Length, source2Length];
    }

    public static string Shorten(this string text, int maxLength)
    {
        var words = text.Split(' ');
        var wordCount = words.Length;
        var length = 0;
        var wordIndex = 0;
        var finished = false;
        var output = new StringBuilder();

        while (length < maxLength && wordIndex < wordCount)
        {
            output.Append(words[wordIndex]);
            length += words[wordIndex].Length;
            wordIndex++;
            if (wordIndex == wordCount)
            {
                finished = true;
            }
        }

        if (finished)
        {
            output.Append("...");
        }
        return output.ToString();
    }

    [GeneratedRegex("[^A-Za-z0-9]")]
    private static partial Regex ToLowerAlphaNumFilterRegex();
}