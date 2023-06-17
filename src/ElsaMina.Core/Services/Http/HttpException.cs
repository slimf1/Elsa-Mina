using System.Net;

namespace ElsaMina.Core.Services.Http;

public class HttpException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ResponseContent { get; }

    public HttpException(HttpStatusCode statusCode, string content)
    {
        StatusCode = statusCode;
        ResponseContent = content;
    }

    public override string Message => $"{nameof(HttpException)}: {ResponseContent} ({StatusCode})";
}