using System.Net;

namespace ElsaMina.Core.Services.Http;

public class HttpException : Exception
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _responseContent;
    
    public HttpException(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _responseContent = content;
    }

    public override string Message => $"{nameof(HttpException)}: {_responseContent} ({_statusCode})";
}