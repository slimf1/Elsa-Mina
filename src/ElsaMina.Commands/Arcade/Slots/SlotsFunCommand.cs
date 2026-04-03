using System.Collections.Concurrent;
using ElsaMina.Commands.Arcade.Events;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Arcade.Slots;

[NamedCommand("slots", Aliases = ["slotsfun"])]
public class SlotsFunCommand : Command
{
    private const int WIN_BASE_THRESHOLD = 68;
    private const int WIN_THRESHOLD_INCREMENT = 4;
    private const int WIN_MESSAGE_COUNT = 6;
    private const int LOSE_MESSAGE_COUNT = 6;

    private static readonly IReadOnlyList<string> BLACKLISTED_ROOMS = ["franais", "salty", "botdevelopment"];

    private static readonly IReadOnlyList<(string DescriptionKey, string ImageUrl)> SLOT_ENTRIES =
    [
        ("slots_description_bulbasaur", "https://www.shinyhunters.com/images/regular/1.gif"),
        ("slots_description_squirtle", "https://www.shinyhunters.com/images/regular/2.gif"),
        ("slots_description_charmander", "https://www.shinyhunters.com/images/regular/3.gif"),
        ("slots_description_pikachu", "https://www.shinyhunters.com/images/regular/4.gif"),
        ("slots_description_eevee", "https://www.shinyhunters.com/images/regular/5.gif"),
        ("slots_description_snorlax", "https://www.shinyhunters.com/images/regular/6.gif"),
        ("slots_description_dragonite", "https://www.shinyhunters.com/images/regular/7.gif"),
        ("slots_description_mew", "https://www.shinyhunters.com/images/regular/8.gif"),
        ("slots_description_mewtwo", "https://www.shinyhunters.com/images/regular/9.gif"),
        ("slots_description_dialga", "https://www.shinyhunters.com/images/regular/10.gif"),
        ("slots_description_palkia", "https://www.shinyhunters.com/images/regular/11.gif"),
        ("slots_description_giratina", "https://www.shinyhunters.com/images/regular/12.gif"),
    ];

    private static readonly IReadOnlyList<string> WIN_MESSAGE_KEYS =
        Enumerable.Range(0, WIN_MESSAGE_COUNT).Select(index => $"slots_win_{index}").ToList();

    private static readonly IReadOnlyList<string> LOSE_MESSAGE_KEYS =
        Enumerable.Range(0, LOSE_MESSAGE_COUNT).Select(index => $"slots_lose_{index}").ToList();

    private static readonly TimeSpan COOLDOWN_DURATION = TimeSpan.FromDays(1);
    private static readonly ConcurrentDictionary<string, DateTimeOffset> USER_COOLDOWNS = new();

    private readonly IRandomService _randomService;
    private readonly IClockService _clockService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IArcadeEventsService _arcadeEventsService;

    public SlotsFunCommand(IRandomService randomService, IClockService clockService, ITemplatesManager templatesManager,
        IArcadeEventsService arcadeEventsService)
    {
        _randomService = randomService;
        _clockService = clockService;
        _templatesManager = templatesManager;
        _arcadeEventsService = arcadeEventsService;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override Rank RequiredRank => Rank.Regular;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (!context.IsPrivateMessage && BLACKLISTED_ROOMS.Contains(context.RoomId))
        {
            context.Reply($"/pm {context.Sender.UserId}, {context.GetString("slots_blacklisted_room")}");
            return;
        }

        if (!context.IsPrivateMessage && _arcadeEventsService.AreGamesMuted(context.RoomId))
        {
            context.ReplyLocalizedMessage("games_muted_event");
            return;
        }

        if (!context.IsPrivateMessage)
        {
            var now = _clockService.CurrentUtcDateTimeOffset;
            if (USER_COOLDOWNS.TryGetValue(context.Sender.UserId, out var lastUse))
            {
                var remaining = COOLDOWN_DURATION - (now - lastUse);
                if (remaining > TimeSpan.Zero)
                {
                    var hours = (int)remaining.TotalHours;
                    var minutes = remaining.Minutes;
                    context.Reply($"/pm {context.Sender.UserId}, {context.GetString("slots_cooldown", hours, minutes)}");
                    return;
                }
            }

            USER_COOLDOWNS[context.Sender.UserId] = now;
        }

        var resultIndex = _randomService.NextInt(SLOT_ENTRIES.Count);
        var winThreshold = WIN_BASE_THRESHOLD + resultIndex * WIN_THRESHOLD_INCREMENT;

        string imageOne, imageTwo, imageThree, outcomeMessage;

        if (_randomService.NextInt(100) >= winThreshold)
        {
            var winEntry = SLOT_ENTRIES[resultIndex];
            imageOne = imageTwo = imageThree = winEntry.ImageUrl;
            var winAcclaim = context.GetString(_randomService.RandomElement(WIN_MESSAGE_KEYS));
            var description = context.GetString(winEntry.DescriptionKey);
            outcomeMessage = $"{winAcclaim} {context.GetString("slots_win_prize", description)}";
        }
        else
        {
            int indexOne, indexTwo, indexThree;
            do
            {
                indexOne = _randomService.NextInt(SLOT_ENTRIES.Count);
                indexTwo = _randomService.NextInt(SLOT_ENTRIES.Count);
                indexThree = _randomService.NextInt(SLOT_ENTRIES.Count);
            } while (indexOne == indexTwo && indexTwo == indexThree);

            imageOne = SLOT_ENTRIES[indexOne].ImageUrl;
            imageTwo = SLOT_ENTRIES[indexTwo].ImageUrl;
            imageThree = SLOT_ENTRIES[indexThree].ImageUrl;
            outcomeMessage = context.GetString(_randomService.RandomElement(LOSE_MESSAGE_KEYS));
        }

        var template = await _templatesManager.GetTemplateAsync("Arcade/Slots/SlotsFun", new SlotsFunViewModel
        {
            Culture = context.Culture,
            UserName = context.Sender.Name,
            OutcomeMessage = outcomeMessage,
            SlotImageOne = imageOne,
            SlotImageTwo = imageTwo,
            SlotImageThree = imageThree
        });

        context.ReplyHtml(template.RemoveNewlines());
    }
}
