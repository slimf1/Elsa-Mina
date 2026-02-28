namespace ElsaMina.Commands.Users.PlayTimes;

public record TopPlayTimesEntry(int Rank, string UserId, string UserName, TimeSpan PlayTime);
