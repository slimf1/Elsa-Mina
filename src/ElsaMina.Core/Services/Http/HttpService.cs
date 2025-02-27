using System.Text;
using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Http;

public class HttpService : IHttpService
{
    private static readonly HttpClient HTTP_CLIENT = new();

    public async Task<IHttpResponse<TResponse>> PostJson<TRequest, TResponse>(string uri, TRequest dto,
        bool removeFirstCharacterFromResponse = false, IDictionary<string, string> headers = null)
    {
        var serializedJson = JsonConvert.SerializeObject(dto);
        var content = new StringContent(serializedJson, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = content;

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await HTTP_CLIENT.SendAsync(request);
        var stringContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpException(response.StatusCode, stringContent);
        }

        if (removeFirstCharacterFromResponse)
        {
            stringContent = stringContent[1..];
        }

        return new HttpResponse<TResponse>
        {
            StatusCode = response.StatusCode,
            Data = JsonConvert.DeserializeObject<TResponse>(stringContent)
        };
    }

    public async Task<IHttpResponse<TResponse>> PostUrlEncodedForm<TResponse>(string uri,
        IDictionary<string, string> form,
        bool removeFirstCharacterFromResponse = false)
    {
        var content = new FormUrlEncodedContent(form);
        var response = await HTTP_CLIENT.PostAsync(uri, content);
        var stringContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpException(response.StatusCode, stringContent);
        }

        if (removeFirstCharacterFromResponse)
        {
            stringContent = stringContent[1..];
        }

        return new HttpResponse<TResponse>
        {
            StatusCode = response.StatusCode,
            Data = JsonConvert.DeserializeObject<TResponse>(stringContent)
        };
    }

    public async Task<Stream> PostStream<TRequest>(string uri, TRequest dto, IDictionary<string, string> headers = null)
    {
        var serializedJson = JsonConvert.SerializeObject(dto);
        var content = new StringContent(serializedJson, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = content;
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await HTTP_CLIENT.SendAsync(request);
        var stringContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpException(response.StatusCode, stringContent);
        }

        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<IHttpResponse<TResponse>> Get<TResponse>(string uri,
        IDictionary<string, string> queryParams = null, IDictionary<string, string> headers = null)
    {
        if (queryParams != null && queryParams.Count > 0)
        {
            var queryString = string.Join("&", queryParams.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            uri = $"{uri}?{queryString}";
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        } 
        var response = await HTTP_CLIENT.SendAsync(request);
        var stringContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpException(response.StatusCode, stringContent);
        }

        return new HttpResponse<TResponse>
        {
            StatusCode = response.StatusCode,
            Data = JsonConvert.DeserializeObject<TResponse>(stringContent)
        };
    }

    public Task<Stream> GetStream(string uri)
    {
        return HTTP_CLIENT.GetStreamAsync(uri);
    }
}