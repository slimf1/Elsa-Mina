using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Rpplf;

[NamedCommand("gen9rpplf")]
public class Gen9RpplfCommand : RpplfTournamentCommand
{
    protected override string Format => "gen9ubers";
    protected override string TourName => "[Gen 9] RPPLF";
    protected override string TeamsName => "gen9rpplf";
    protected override string TourRules =>
        "+Razor Fang, -Light Clay, -Dire Claw, -Shadow Tag, -Shed Tail, Item Clause=1, " +
        "-Mewtwo, -Lugia, -Ho-Oh, -Groudon, -Kyogre, -Rayquaza, -Deoxys, -Deoxys-Attack, " +
        "-Dialga, -Dialga-Origin, -Palkia, -Palkia-Origin, -Giratina, -Giratina-Origin, " +
        "-Arceus, -Kyurem-White, -Kyurem-Black, -Reshiram, -Zekrom, -Solgaleo, -Lunala, " +
        "-Necrozma-Dusk-Mane, -Necrozma-Dawn-Wings, -Magearna, -Zacian-Crowned, -Zacian, " +
        "-Zamazenta-Crowned, -Eternatus, -Spectrier, -Calyrex-Ice, -Flutter Mane, -Chi-Yu, " +
        "-Koraidon, +Deoxys-Speed, +Deoxys-Defense, -Chien Pao, -Terapagos-Stellar, " +
        "+Terapagos, +Terapagos-Terastal";
}
