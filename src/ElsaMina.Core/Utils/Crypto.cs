using System.Security.Cryptography;
using System.Text;

namespace ElsaMina.Core.Utils;

public static class Crypto
{
    public static string ToMd5Digest(this string text)
    {
        var stringBuilder = new StringBuilder();
        foreach (var octet in MD5.HashData(Encoding.UTF8.GetBytes(text)))
        {
            stringBuilder.Append(octet.ToString("x2").ToLower());
        }

        return stringBuilder.ToString();
    }
}