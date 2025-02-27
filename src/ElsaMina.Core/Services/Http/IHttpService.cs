namespace ElsaMina.Core.Services.Http;

public interface IHttpService // TODO : refactor this shit
{
    public Task<IHttpResponse<TResponse>> PostJson<TRequest, TResponse>(string uri, TRequest dto,
        bool removeFirstCharacterFromResponse = false, IDictionary<string, string> headers = null);
    public Task<Stream> PostStream<TRequest>(string uri, TRequest dto, IDictionary<string, string> headers = null);
    public Task<IHttpResponse<TResponse>> PostUrlEncodedForm<TResponse>(string uri, IDictionary<string, string> form,
        bool removeFirstCharacterFromResponse = false);
    public Task<IHttpResponse<TResponse>> Get<TResponse>(string uri, IDictionary<string, string> queryParams = null, IDictionary<string, string> headers = null);
    Task<Stream> GetStream(string uri);
}