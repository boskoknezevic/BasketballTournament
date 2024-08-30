using BasketballTournament.Methods;
using BasketballTournament.Models;
using static BasketballTournament.Methods.ImportTeams;
using static BasketballTournament.Methods.PrintingMethods;
using static BasketballTournament.Methods.ProbabilityCalculator;
using static BasketballTournament.Methods.MatchMethods;
using static System.Console;

List<Group> groups = ImportTeamsFromJSON();
WriteLine("GROUPS");
PrintGroup(groups);
WriteLine();

ReadProbabilityFromJSON(groups);

List<Round> rounds = CreateASchedule(groups);
WriteLine("SCHEDULE");
PrintSchedule(rounds);
WriteLine();

rounds = SimulateRound(rounds);
WriteLine("SIMULATED GROUP STAGE");
PrintSchedule(rounds);
WriteLine();

WriteLine("TABLES");
var allGroupStats = CalculateAllGroupStatistics(groups);
foreach (var group in allGroupStats)
{
    PrintStatistics(group);
    WriteLine();
}
WriteLine();

AdvanceFromGroups(groups, allGroupStats);
PrintGroup(groups);

WriteLine("QUALIFIED TEAMS");
PrintQualifiedTeams(groups);
var newGroups = CalculateTop8(groups, allGroupStats);

WriteLine("BEST 8 STATISTICS");
PrintGroup(newGroups);
WriteLine("BEST 8");
PrintQualifiedTeams(newGroups);

var pots = CreateKnockoutGroups(newGroups);
PrintQualifiedTeams(pots);
WriteLine();

var quarterFinalMatches = CreateQuarterFinals(pots);
PrintSchedule(quarterFinalMatches);

var simulatedQuarterFinal = SimulateRound(quarterFinalMatches);
PrintSchedule(simulatedQuarterFinal);
WriteLine();

var semiFinals = CreateSemiFinals(quarterFinalMatches);
PrintSchedule(semiFinals);

var simulatedSemiFinals = SimulateRound(semiFinals);
PrintSchedule(simulatedSemiFinals);
WriteLine();

var finals = CreateFinalsAndThirdPlaceGame(semiFinals);
PrintSchedule(finals);

var simulatedFinals = SimulateRound(finals);
PrintSchedule(simulatedFinals);
WriteLine();

PrintTheMedals(simulatedFinals);