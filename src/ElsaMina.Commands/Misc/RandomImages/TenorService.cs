using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Misc.RandomImages;

public class TenorService : ITenorService
{
    private const string TENOR_SEARCH_URL = "https://g.tenor.com/v2/search";
    private const int RESULT_LIMIT = 10;

    private readonly IHttpService _httpService;
    private readonly IConfiguration _configuration;
    private readonly IRandomService _randomService;

    public TenorService(IHttpService httpService, IConfiguration configuration, IRandomService randomService)
    {
        _httpService = httpService;
        _configuration = configuration;
        _randomService = randomService;
    }

    public async Task<TenorMediaInfo> GetRandomMediaAsync(string query, string mediaFormat,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration.TenorApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("Tenor API key is empty.");
            return null;
        }

        var queryParams = new Dictionary<string, string>
        {
            ["q"] = query,
            ["key"] = apiKey,
            ["limit"] = RESULT_LIMIT.ToString(),
            ["media_filter"] = "minimal"
        };

        try
        {
            var response = await _httpService.GetAsync<TenorResponseDto>(TENOR_SEARCH_URL, queryParams,
                cancellationToken: cancellationToken);
            var results = response.Data?.Results;
            if (results == null || results.Count == 0)
            {
                return null;
            }

            var selected = results[_randomService.NextInt(results.Count)];
            if (selected.MediaFormats == null || !selected.MediaFormats.TryGetValue(mediaFormat, out var media))
            {
                return null;
            }

            return new TenorMediaInfo(media.Url, media.Dims[0], media.Dims[1]);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to fetch Tenor media for query: {Query}", query);
            return null;
        }
    }
}