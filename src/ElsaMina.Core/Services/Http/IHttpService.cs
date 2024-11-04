namespace ElsaMina.Core.Services.Http;

public interface IHttpService
{
    public Task<HttpResponse<TResponse>> PostJson<TRequest, TResponse>(string uri, TRequest dto,
        bool removeFirstCharacterFromResponse = false);
    public Task<HttpResponse<TResponse>> PostUrlEncodedForm<TResponse>(string uri, IDictionary<string, string> form,
        bool removeFirstCharacterFromResponse = false);
    public Task<HttpResponse<TResponse>> Get<TResponse>(string uri, IDictionary<string, string> queryParams = null);
}