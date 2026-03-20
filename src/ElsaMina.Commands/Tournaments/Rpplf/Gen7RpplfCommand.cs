using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Rpplf;

[NamedCommand("gen7rpplf")]
public class Gen7RpplfCommand : RpplfTournamentCommand
{
    protected override string Format => "gen7ubers";
    protected override string TourName => "[Gen 7] RPPLF";
    protected override string TeamsName => "gen7rpplf";
    protected override int? AutoDq => 5;
    protected override string TourRules =>
        "-Light Clay, -Shadow Tag, -King's Rock, -Soul Dew, Item Clause=1, " +
        "-Mewtwo, -Lugia, -Ho-Oh, -Groudon, -Kyogre, -Rayquaza, -Deoxys, -Deoxys-Attack, " +
        "-Deoxys-Defense, -Dialga, -Palkia, -Giratina, -Giratina-Origin, -Darkrai, " +
        "-Shaymin-Sky, -Arceus, -Reshiram, -Zekrom, -Kyurem-White, -Xerneas, -Yveltal, " +
        "-Zygarde-Complete, -Pheromosa, -Solgaleo, -Lunala, -Necrozma-Dusk-Mane, " +
        "-Necrozma-Dawn-Wings, -Marshadow, -Blazikenite, -Gengarite, -Salamencite, " +
        "+Zygarde, +Zygarde-10%, +Deoxys-Speed, -Power Construct";
}
