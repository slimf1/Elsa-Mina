using Newtonsoft.Json.Linq;

namespace ElsaMina.DataAccess.Utils;

public static class DbConfigProvider
{
    private static string _cachedConnectionString;
    
    public static string GetConnectionString()
    {
        if (_cachedConnectionString != null)
        {
            return _cachedConnectionString;
        }
        
        var configurationFile = Environment.GetEnvironmentVariable("ELSA_MINA_ENV") switch
        {
            "prod" => "prod.config.json",
            _ => "dev.config.json"
        };
        
        using (var reader = new StreamReader(Path.Join("Config", configurationFile)))
        {
            _cachedConnectionString = JObject.Parse(reader.ReadToEnd()).GetValue("ConnectionString")?.Value<string>()
                                      ?? string.Empty;
        }

        return _cachedConnectionString;
    }
}