using Newtonsoft.Json;

namespace ElsaMina.DataAccess.Utils;

public static class DbConfigProvider
{
    private static DbConfig _cachedDbConfig;
    
    public static DbConfig GetDbConfig()
    {
        if (_cachedDbConfig != null)
        {
            return _cachedDbConfig;
        }
        
        var configurationFile = Environment.GetEnvironmentVariable("ELSA_MINA_ENV") switch
        {
            "prod" => "prod.dbconfig.json",
            _ => "dev.dbconfig.json"
        };
        
        using (var reader = new StreamReader(Path.Join("Config", configurationFile)))
        {
            _cachedDbConfig = JsonConvert.DeserializeObject<DbConfig>(reader.ReadToEnd());
        }

        return _cachedDbConfig;
    }
}