using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Http;

public class HttpService : IHttpService
{
    private readonly SocketsHttpHandler _handler = new()
    {
        AutomaticDecompression = DecompressionMethods.All,
        PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        ConnectCallback = async (context, cancellationToken) =>
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };
            await socket.ConnectAsync(context.DnsEndPoint.Host, context.DnsEndPoint.Port, cancellationToken);
            return new NetworkStream(socket, ownsSocket: true);
        }
    };

    private readonly HttpClient _httpClient;

    public HttpService()
    {
        _httpClient = new HttpClient(_handler)
        {
            Timeout = TimeSpan.FromSeconds(30),
        };
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "image/avif,image/webp,image/apng,*/*;q=0.8");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://imgur.com/");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-dest", "image");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-mode", "no-cors");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-site", "cross-site");
    }

    public async Task<IHttpResponse<TResponse>> PostJsonAsync<TRequest, TResponse>(
        string uri,
        TRequest dto,
        bool removeFirstCharacterFromResponse = false,
        IDictionary<string, string> headers = null,
        bool isRaw = false,
        CancellationToken cancellationToken = default)
    {
        var content = CreateJsonContent(dto);
        var response = await SendRequestAsync<TResponse>(HttpMethod.Post, uri, content, headers,
            removeFirstCharacterFromResponse, isRaw, cancellationToken);
        return response;
    }

    public async Task<IHttpResponse<TResponse>> PostUrlEncodedFormAsync<TResponse>(
        string uri,
        IDictionary<string, string> form,
        bool removeFirstCharacterFromResponse = false,
        bool isRaw = false,
        CancellationToken cancellationToken = default)
    {
        var content = new FormUrlEncodedContent(form);
        var response = await SendRequestAsync<TResponse>(HttpMethod.Post, uri, content, null,
            removeFirstCharacterFromResponse, isRaw, cancellationToken);
        return response;
    }

    public async Task<Stream> DownloadContentWithPostAsync<TRequest>(
        string uri,
        TRequest dto,
        IDictionary<string, string> headers = null,
        CancellationToken cancellationToken = default)
    {
        var content = CreateJsonContent(dto);
        var response = await SendRequestAsync(HttpMethod.Post, uri, content, headers, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    public async Task<IHttpResponse<TResponse>> GetAsync<TResponse>(
        string uri,
        IDictionary<string, string> queryParams = null,
        IDictionary<string, string> headers = null,
        bool removeFirstCharacterFromResponse = false,
        bool isRaw = false,
        CancellationToken cancellationToken = default)
    {
        if (queryParams != null && queryParams.Count > 0)
        {
            uri =
                $"{uri}?{string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"))}";
        }

        var response = await SendRequestAsync<TResponse>(HttpMethod.Get, uri, null, headers,
            removeFirstCharacterFromResponse, isRaw, cancellationToken);
        return response;
    }

    public Task<Stream> GetStreamAsync(string uri, CancellationToken cancellationToken = default)
    {
        return _httpClient.GetStreamAsync(uri, cancellationToken);
    }

    private static StringContent CreateJsonContent<T>(T dto)
    {
        var serializedJson = JsonConvert.SerializeObject(dto);
        return new StringContent(serializedJson, Encoding.UTF8, "application/json");
    }

    private static void AddHeaders(HttpRequestMessage request, IDictionary<string, string> headers)
    {
        if (headers == null)
        {
            return;
        }

        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }
    }

    private async Task<IHttpResponse<TResponse>> SendRequestAsync<TResponse>(
        HttpMethod method,
        string uri,
        HttpContent content,
        IDictionary<string, string> headers,
        bool removeFirstCharacterFromResponse,
        bool isRaw,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, uri);
        request.Content = content;
        AddHeaders(request, headers);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

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
            Data = isRaw ? (TResponse)(object)stringContent : JsonConvert.DeserializeObject<TResponse>(stringContent)
        };
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        HttpMethod method,
        string uri,
        HttpContent content,
        IDictionary<string, string> headers,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, uri);
        request.Content = content;
        AddHeaders(request, headers);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpException(response.StatusCode, stringContent);
        }

        return response;
    }
}