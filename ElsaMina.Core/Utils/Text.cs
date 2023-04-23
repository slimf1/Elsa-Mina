using System.Text.RegularExpressions;

namespace ElsaMina.Core.Utils;

public static class Text
{
    public static string ToLowerAlphaNum(this string text)
    {
        return Regex.Replace(text.ToLower(), @"[^A-Za-z0-9]", "");
    }
}