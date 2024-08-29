using BasketballTournament.Methods;
using BasketballTournament.Models;
using BasketballTournament.Services;

List<Group> groups = ImportTeams.ImportTeamsFromJSON();
Console.WriteLine("GROUPS");
PrintingMethods.PrintGroup(groups);
Console.WriteLine();

List<Round> rounds = MatchMethods.CreateASchedule(groups);
Console.WriteLine("SCHEDULE");
PrintingMethods.PrintSchedule(rounds);
Console.WriteLine();

rounds = MatchMethods.SimulateRound(rounds);
Console.WriteLine("SIMULATED GROUP STAGE");
PrintingMethods.PrintSchedule(rounds);
Console.WriteLine();

Console.WriteLine("TABLES");
var allGroupStats = MatchMethods.CalculateAllGroupStatistics(groups);
foreach (var group in allGroupStats)
{
    PrintingMethods.PrintStatistics(group);
    Console.WriteLine();
}
Console.WriteLine();


MatchMethods.AdvanceFromGroups(groups, allGroupStats);
PrintingMethods.PrintGroup(groups);

Console.WriteLine("QUALIFIED TEAMS");
PrintingMethods.PrintQualifiedTeams(groups);
var newGroups = MatchMethods.CalculateTop8(groups, allGroupStats);

Console.WriteLine("BEST 8 STATISTICS");
PrintingMethods.PrintGroup(newGroups);
Console.WriteLine("BEST 8");
PrintingMethods.PrintQualifiedTeams(newGroups);

var pots = MatchMethods.CreateKnockoutGroups(newGroups);
PrintingMethods.PrintQualifiedTeams(pots);
Console.WriteLine();

var quarterFinalMatches = MatchMethods.CreateQuarterFinals(pots);
PrintingMethods.PrintSchedule(quarterFinalMatches);

var simulatedQuarterFinal = MatchMethods.SimulateRound(quarterFinalMatches);
PrintingMethods.PrintSchedule(simulatedQuarterFinal);
Console.WriteLine();

var semiFinals = MatchMethods.CreateSemiFinals(quarterFinalMatches);
PrintingMethods.PrintSchedule(semiFinals);

var simulatedSemiFinals = MatchMethods.SimulateRound(semiFinals);
PrintingMethods.PrintSchedule(simulatedSemiFinals);
Console.WriteLine();

var finals = MatchMethods.CreateFinalsAndThirdPlaceGame(semiFinals);
PrintingMethods.PrintSchedule(finals);

var simulatedFinals = MatchMethods.SimulateRound(finals);
PrintingMethods.PrintSchedule(simulatedFinals);
Console.WriteLine();

PrintingMethods.PrintTheMedals(simulatedFinals);