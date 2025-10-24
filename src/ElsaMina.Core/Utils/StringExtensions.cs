using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ElsaMina.Core.Utils;

public static class StringExtensions
{
    private static readonly Regex ALPHA_NUMERIC_FILTER_REGEX = new("[^A-Za-z0-9]",
        RegexOptions.Compiled, Constants.REGEX_MATCH_TIMEOUT);

    private static readonly Regex WHITESPACE_BETWEEN_TAGS_REGEX =
        new(@"\s*(<[^>]+>)\s*", RegexOptions.Compiled, Constants.REGEX_MATCH_TIMEOUT);

    private static readonly Regex IMAGE_LINK_REGEX = new("(http)?s?:(//[^\"']*.(?:png|jpg|jpeg|gif|png|svg))",
        RegexOptions.Compiled, Constants.REGEX_MATCH_TIMEOUT);

    public static string ToLowerAlphaNum(this string text)
    {
        return ALPHA_NUMERIC_FILTER_REGEX.Replace(text.ToLower(), string.Empty);
    }

    public static string RemoveNewlines(this string text)
    {
        return text.Replace("\n", string.Empty);
    }

    public static string RemoveWhitespacesBetweenTags(this string text)
    {
        return WHITESPACE_BETWEEN_TAGS_REGEX.Replace(text, "$1");
    }

    public static string Capitalize(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return text[0].ToString().ToUpper() + text[1..];
    }

    public static bool ToBoolean(this string text)
    {
        return text.Trim().ToLower() is "true" or "y" or "t" or "1" or "on";
    }

    public static string Shorten(this string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || maxLength <= 0)
        {
            return string.Empty;
        }

        var words = text.Split(' ');
        var output = new StringBuilder();
        var length = 0;

        foreach (var word in words)
        {
            if (length + word.Length > maxLength)
            {
                break;
            }

            if (output.Length > 0)
            {
                output.Append(' ');
                length++;
            }

            output.Append(word);
            length += word.Length;
        }

        if (length < text.Length)
        {
            output.Append("...");
        }

        return output.ToString();
    }

    public static string ToMd5Digest(this string text)
    {
        var stringBuilder = new StringBuilder();
        foreach (var octet in MD5.HashData(Encoding.UTF8.GetBytes(text)))
        {
            stringBuilder.Append(octet.ToString("x2").ToLower());
        }

        return stringBuilder.ToString();
    }

    public static bool IsValidImageLink(this string link) =>
        !string.IsNullOrWhiteSpace(link) && IMAGE_LINK_REGEX.IsMatch(link);

    /// <remarks>
    /// Credit : https://gist.github.com/Davidblkx/e12ab0bb2aff7fd8072632b396538560
    /// </remarks>
    public static int LevenshteinDistance(this string source, string other)
    {
        var source1Length = source.Length;
        var source2Length = other.Length;

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
                var cost = other[j - 1] == source[i - 1] ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[source1Length, source2Length];
    }

    public static string RemoveExtension(this string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return fileName[..^extension.Length];
    }
}