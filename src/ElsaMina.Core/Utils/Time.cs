namespace ElsaMina.Core.Utils;

public static class Time
{
    public static DateTime GetDateTimeFromUnixTime(long unixTime)
    {
        return DateTime.UnixEpoch.AddSeconds(unixTime);
    }
}
