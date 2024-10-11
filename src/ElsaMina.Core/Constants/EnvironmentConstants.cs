namespace ElsaMina.Core.Constants;

public static class EnvironmentConstants
{
#if DEBUG
    public const bool IS_DEBUG = true;
#else
    public const bool IS_DEBUG = false;
#endif
}