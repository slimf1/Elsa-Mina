using System.Net;

namespace ElsaMina.Core.Services.Http;

public interface IHttpResponse<T>
{
    T Data { get; init; }
    HttpStatusCode StatusCode { get; init; }
}