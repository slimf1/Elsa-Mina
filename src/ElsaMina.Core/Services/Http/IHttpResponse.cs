using System.Net;

namespace ElsaMina.Core.Services.Http;

public interface IHttpResponse<out T>
{
    T Data { get; }
    HttpStatusCode StatusCode { get; }
}