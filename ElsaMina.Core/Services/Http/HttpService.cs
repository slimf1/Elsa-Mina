namespace ElsaMina.Core.Services.Http;

public class HttpService : IHttpService
{
    private static readonly HttpClient HTTP_CLIENT = new();

    public async Task<string> PostFormAsync(string url, IDictionary<string, string> form)
    {
        var content = new FormUrlEncodedContent(form);
        var response = await HTTP_CLIENT.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }
}