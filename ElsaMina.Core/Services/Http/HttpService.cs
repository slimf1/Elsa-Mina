﻿using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Http;

public class HttpService : IHttpService
{
    private static readonly HttpClient HTTP_CLIENT = new();

    public async Task<TResponse> PostJson<TRequest, TResponse>(string uri, TRequest dto,
        bool removeFirstCharacterFromResponse = false)
    {
        var serializedJson = JsonConvert.SerializeObject(dto);
        var content = new StringContent(serializedJson);
        var response = await HTTP_CLIENT.PostAsync(uri, content);
        var stringContent = await response.Content.ReadAsStringAsync();
        if (removeFirstCharacterFromResponse)
        {
            stringContent = stringContent[1..];
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpException(response.StatusCode, stringContent);
        }
        return JsonConvert.DeserializeObject<TResponse>(stringContent);
    }
    
    public async Task<TResponse> PostUrlEncodedForm<TResponse>(string uri, IDictionary<string, string> form,
        bool removeFirstCharacterFromResponse = false)
    {
        var content = new FormUrlEncodedContent(form);
        var response = await HTTP_CLIENT.PostAsync(uri, content);
        var stringContent = await response.Content.ReadAsStringAsync();
        if (removeFirstCharacterFromResponse)
        {
            stringContent = stringContent[1..];
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpException(response.StatusCode, stringContent);
        }
        return JsonConvert.DeserializeObject<TResponse>(stringContent);
    }
}