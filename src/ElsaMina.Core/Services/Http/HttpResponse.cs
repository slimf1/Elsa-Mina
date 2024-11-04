using System.Net;

namespace ElsaMina.Core.Services.Http;

public class HttpResponse<T> : IHttpResponse<T>
{
    public T Data { get; init; }
    public HttpStatusCode StatusCode { get; init; }
}