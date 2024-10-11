namespace ElsaMina.Core.Constants;

public static class EnvironmentConstants
{
    public const string ENVIRONMENT_VARIABLE_NAME = "ELSA_MINA_ENV";
    
    public const string DEV = "dev";
    public const string PROD = "prod";
    
#if DEBUG
    public const bool IS_DEBUG = true;
#else
    public const bool IS_DEBUG = false;
#endif
}