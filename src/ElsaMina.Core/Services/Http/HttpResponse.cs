using System.Net;

namespace ElsaMina.Core.Services.Http;

public class HttpResponse<T>
{
    public T Data { get; init; }
    public HttpStatusCode StatusCode { get; init; }
}