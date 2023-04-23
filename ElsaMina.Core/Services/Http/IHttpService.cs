namespace ElsaMina.Core.Services.Http;

public interface IHttpService
{
    Task<string> PostFormAsync(string url, IDictionary<string, string> form);
}