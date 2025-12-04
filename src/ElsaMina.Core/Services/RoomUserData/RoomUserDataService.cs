using System.Collections.Concurrent;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Core.Services.RoomUserData;

public class RoomUserDataService : IRoomUserDataService
{
    private const int TITLE_MAX_LENGTH = 450;
    private const int JOIN_PHRASE_MAX_LENGTH = 300;

    private readonly IBotDbContextFactory _dbContextFactory;

    private readonly ConcurrentDictionary<Tuple<string, string>, string> _joinPhrases = new();

    public RoomUserDataService(IBotDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public IReadOnlyDictionary<Tuple<string, string>, string> JoinPhrases => _joinPhrases;

    public async Task<RoomUser> GetUserData(
        string roomId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await GetUserAndCreateIfNotExistsAsync(roomId, userId, cancellationToken);
    }

    public async Task InitializeJoinPhrasesAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var usersWithJoinPhrase = await dbContext.RoomUsers
            .Where(userData => !string.IsNullOrEmpty(userData.JoinPhrase))
            .ToListAsync(cancellationToken);

        foreach (var userData in usersWithJoinPhrase)
        {
            _joinPhrases[Tuple.Create(userData.Id, userData.RoomId)] = userData.JoinPhrase!;
        }
    }

    private async Task<RoomUser> GetUserAndCreateIfNotExistsAsync(
        string roomId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existingUser = await dbContext.RoomUsers.FindAsync([userId, roomId], cancellationToken);

        if (existingUser != null)
        {
            return existingUser;
        }

        var newUser = new RoomUser
        {
            Id = userId,
            RoomId = roomId,
            PlayTime = TimeSpan.Zero
        };

        await dbContext.RoomUsers.AddAsync(newUser, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return newUser;
    }

    public async Task GiveBadgeToUserAsync(
        string roomId,
        string userId,
        string badgeId,
        CancellationToken cancellationToken = default)
    {
        await GetUserAndCreateIfNotExistsAsync(roomId, userId, cancellationToken);

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var badgeHolding = new BadgeHolding
        {
            BadgeId = badgeId,
            RoomId = roomId,
            UserId = userId
        };

        await dbContext.BadgeHoldings.AddAsync(badgeHolding, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task TakeBadgeFromUserAsync(
        string roomId,
        string userId,
        string badgeId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existing = await dbContext.BadgeHoldings
            .FirstOrDefaultAsync(
                x => x.BadgeId == badgeId && x.UserId == userId && x.RoomId == roomId,
                cancellationToken);

        if (existing == null)
            throw new ArgumentException("Badge not found");

        dbContext.BadgeHoldings.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetUserTitleAsync(
        string roomId,
        string userId,
        string title,
        CancellationToken cancellationToken = default)
    {
        if (title != null && title.Length > TITLE_MAX_LENGTH)
        {
            throw new ArgumentException("Title too long");
        }

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await GetUserAndCreateIfNotExistsAsync(roomId, userId, cancellationToken);

        user.Title = title;

        dbContext.RoomUsers.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetUserAvatarAsync(
        string roomId,
        string userId,
        string avatar,
        CancellationToken cancellationToken = default)
    {
        if (avatar != null && !avatar.IsValidImageLink())
        {
            throw new ArgumentException("Invalid URL");
        }

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await GetUserAndCreateIfNotExistsAsync(roomId, userId, cancellationToken);

        user.Avatar = avatar;
        dbContext.RoomUsers.Update(user);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetUserJoinPhraseAsync(
        string roomId,
        string userId,
        string joinPhrase,
        CancellationToken cancellationToken = default)
    {
        if (joinPhrase != null && joinPhrase.Length > JOIN_PHRASE_MAX_LENGTH)
        {
            throw new ArgumentException("Join phrase too long");
        }

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await GetUserAndCreateIfNotExistsAsync(roomId, userId, cancellationToken);

        user.JoinPhrase = joinPhrase;

        var key = Tuple.Create(user.Id, user.RoomId);

        if (string.IsNullOrEmpty(joinPhrase))
        {
            _joinPhrases.Remove(key, out _);
        }
        else
        {
            _joinPhrases[key] = joinPhrase;
        }

        dbContext.RoomUsers.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task IncrementUserPlayTime(
        string roomId,
        string userId,
        TimeSpan additionalPlayTime,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await GetUserAndCreateIfNotExistsAsync(roomId, userId, cancellationToken);
        user.PlayTime += additionalPlayTime;
        dbContext.RoomUsers.Update(user);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}