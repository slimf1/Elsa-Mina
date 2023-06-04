namespace ElsaMina.Core.Services.Http;

public interface IHttpService
{
    public Task<TResponse> PostJson<TRequest, TResponse>(string uri, TRequest dto,
        bool removeFirstCharacterFromResponse = false);
    public Task<TResponse> PostUrlEncodedForm<TResponse>(string uri, IDictionary<string, string> form,
        bool removeFirstCharacterFromResponse = false);
    public Task<TResponse> Get<TResponse>(string uri);
}