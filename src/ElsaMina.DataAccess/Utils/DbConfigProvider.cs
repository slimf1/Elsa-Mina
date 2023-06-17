using Newtonsoft.Json;

namespace ElsaMina.DataAccess.Utils;

public static class DbConfigProvider
{
    private static DbConfig _currentDbConfig;
    
    public static DbConfig GetDbConfig()
    {
        if (_currentDbConfig != null)
        {
            return _currentDbConfig;
        }
        
        var configurationFile = Environment.GetEnvironmentVariable("ELSA_MINA_ENV") switch
        {
            "prod" => "prod.dbconfig.json",
            _ => "dev.dbconfig.json"
        };
        
        using (var reader = new StreamReader(Path.Join("Config", configurationFile)))
        {
            _currentDbConfig = JsonConvert.DeserializeObject<DbConfig>(reader.ReadToEnd());
        }

        return _currentDbConfig;
    }
}