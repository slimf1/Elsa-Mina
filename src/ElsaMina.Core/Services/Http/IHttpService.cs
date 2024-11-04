namespace ElsaMina.Core.Services.Http;

public interface IHttpService
{
    public Task<IHttpResponse<TResponse>> PostJson<TRequest, TResponse>(string uri, TRequest dto,
        bool removeFirstCharacterFromResponse = false);
    public Task<IHttpResponse<TResponse>> PostUrlEncodedForm<TResponse>(string uri, IDictionary<string, string> form,
        bool removeFirstCharacterFromResponse = false);
    public Task<IHttpResponse<TResponse>> Get<TResponse>(string uri, IDictionary<string, string> queryParams = null);
}