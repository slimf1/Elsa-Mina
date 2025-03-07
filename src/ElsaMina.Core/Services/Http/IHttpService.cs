namespace ElsaMina.Core.Services.Http;

public interface IHttpService // TODO : refactor this shit
{
    public Task<IHttpResponse<TResponse>> PostJsonAsync<TRequest, TResponse>(string uri, TRequest dto,
        bool removeFirstCharacterFromResponse = false, IDictionary<string, string> headers = null);

    public Task<Stream> PostStreamAsync<TRequest>(string uri, TRequest dto, IDictionary<string, string> headers = null);

    public Task<IHttpResponse<TResponse>> PostUrlEncodedFormAsync<TResponse>(string uri, IDictionary<string, string> form,
        bool removeFirstCharacterFromResponse = false);

    public Task<IHttpResponse<TResponse>> GetAsync<TResponse>(string uri, IDictionary<string, string> queryParams = null,
        IDictionary<string, string> headers = null, bool removeFirstCharacterFromResponse = false,
        CancellationToken cancellationToken = default);

    Task<Stream> GetStreamAsync(string uri);
}