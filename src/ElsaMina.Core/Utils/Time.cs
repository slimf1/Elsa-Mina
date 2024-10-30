namespace ElsaMina.Core.Utils;

public static class Time
{
    private static readonly DateTime EPOCH = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime GetDateTimeFromUnixTime(long unixTime)
    {
        return EPOCH.AddSeconds(unixTime);
    }
}