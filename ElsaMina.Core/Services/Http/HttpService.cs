using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Http;

public class HttpService : IHttpService
{
    private static readonly HttpClient HTTP_CLIENT = new();

    public async Task<TResponse> Post<TRequest, TResponse>(string uri, TRequest dto)
    {
        var serializedJson = JsonConvert.SerializeObject(dto);
        var content = new StringContent(serializedJson);
        var response = await HTTP_CLIENT.PostAsync(uri, content);
        var stringContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpException(response.StatusCode, stringContent);
        }
        return JsonConvert.DeserializeObject<TResponse>(stringContent);
    }

    public async Task<string> PostFormAsync(string url, IDictionary<string, string> form)
    {
        var content = new FormUrlEncodedContent(form);
        var response = await HTTP_CLIENT.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }
}