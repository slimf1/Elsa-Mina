using System.Text.RegularExpressions;

namespace ElsaMina.Core.Utils;

public static class Text
{
    public static string ToLowerAlphaNum(this string text)
    {
        return Regex.Replace(text.ToLower(), @"[^A-Za-z0-9]", "");
    }

    public static string RemoveNewlines(this string text)
    {
        return text.Replace("\n", string.Empty);
    }

    public static string Capitalize(this string text)
    {
        return text[0].ToString().ToUpper() + text[1..];
    }
}