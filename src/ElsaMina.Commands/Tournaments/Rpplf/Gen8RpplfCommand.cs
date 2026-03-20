using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Rpplf;

[NamedCommand("gen8rpplf")]
public class Gen8RpplfCommand : RpplfTournamentCommand
{
    protected override string Format => "gen8ubers";
    protected override string TourName => "[Gen 8] RPPLF";
    protected override string TeamsName => "gen8rpplf";
    protected override int? AutoDq => 5;
    protected override string TourRules =>
        "+Razor Fang, -Light Clay, -Moody, -King's Rock, Item Clause=1, " +
        "-Mewtwo, -Lugia, -Ho-Oh, -Groudon, -Kyogre, -Rayquaza, -Dialga, -Palkia, " +
        "-Giratina, -Giratina-Origin, -Reshiram, -Zekrom, -Kyurem-White, -Kyurem-Black, " +
        "-Genesect, -Xerneas, -Yveltal, -Zygarde, -Solgaleo, -Lunala, " +
        "-Necrozma-Dusk-Mane, -Necrozma-Dawn-Wings, -Magearna, -Pheromosa, -Marshadow, " +
        "-Dracovish, -Zacian-Crowned, -Zacian, -Zamazenta, -Zamazenta-Crowned, -Eternatus, " +
        "-Calyrex-Ice, -Calyrex-Shadow, !Dynamax Clause, +Zygarde-10%, +Urshifu-Rapid-Strike";
}
