using BasketballTournament.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = BasketballTournament.Models.Group;
using Match = BasketballTournament.Models.Match;

namespace BasketballTournament.Methods
{
    public static class PrintingMethods
    {
        public static void PrintTeam(Team team)
        {
            Console.WriteLine($"  Team: {team.Name,20}, ISOCode: {team.IsoCode,20}, FIBARanking: {team.FibaRanking,20}");
            var teamStats = MatchMethods.CalculateTeamStatistic(team);
            if (!(teamStats.PointsScored == 0 && teamStats.PointsConceded == 0 && teamStats.Points == 0))
            {
                Console.WriteLine($"{"Points Scored:",-20} {teamStats.PointsScored,-10} {"Points Conceded:",-20} {teamStats.PointsConceded,-10} {"Points Difference:",-20} {teamStats.PointsScored - teamStats.PointsConceded,-10} {"Points:",-10} {teamStats.Points,-5}");
                Console.WriteLine();
            }
        }

        public static void PrintMatch(Match match)
        {
            if (match.HomeScore == 0 && match.AwayScore == 0) { Console.WriteLine($"{match.HomeTeam.Name,20} - {match.AwayTeam.Name,-20}"); }
            else { Console.WriteLine($"{match.HomeTeam.Name,20} - {match.AwayTeam.Name,-20} {match.HomeScore,20} : {match.AwayScore,-20}      {match.Date}"); }
        }

        public static void PrintGroup(List<Group> groups)
        {
            foreach (var group in groups)
            {
                Console.WriteLine($"Group: {group.Name}");
                foreach (var team in group.Teams)
                {
                    PrintTeam(team);
                }
            }
        }

        public static void PrintSchedule(List<Round> rounds)
        {
            foreach (var round in rounds)
            {
                if (round.RoundNumber > 0 && round.RoundNumber < 4)
                {
                    Console.WriteLine($"{round.RoundNumber}. ROUND");
                    foreach (var match in round.Matches)
                        PrintMatch(match);
                }
                else if (round.RoundNumber == 4)
                {
                    Console.WriteLine($"QUARTERFINALS");
                    foreach (var match in round.Matches)
                        PrintMatch(match);
                }
                else if (round.RoundNumber == 5)
                {
                    Console.WriteLine($"SEMIFINALS");
                    foreach (var match in round.Matches)
                        PrintMatch(match);
                }
                else if (round.RoundNumber == 6)
                {
                    Console.WriteLine("THIRD PLACE GAME");
                    foreach (var match in round.Matches)
                        PrintMatch(match);
                }
                else if (round.RoundNumber == 7)
                {
                    Console.WriteLine("FINAL GAME");
                    foreach (var match in round.Matches)
                        PrintMatch(match);
                    Console.WriteLine();
                }
            }
        }

        public static void PrintStatistics(KeyValuePair<char, Dictionary<string, (int PointsScored, int PointsConceded, int Points)>> teamStats)
        {
            Console.WriteLine(teamStats.Key);
            Console.WriteLine("Team                 | Points Scored | Points Conceded  | Point Difference  | Points");
            Console.WriteLine(new string('-', 90));

            foreach (var stat in teamStats.Value)
            {
                var teamName = stat.Key;
                var stats = stat.Value;
                int pointDifference = stats.PointsScored - stats.PointsConceded;

                Console.WriteLine($"{teamName.PadRight(20)} | {stats.PointsScored.ToString().PadRight(13)} | {stats.PointsConceded.ToString().PadRight(16)} | {pointDifference.ToString().PadRight(17)} | {stats.Points}");
            }
        }

        public static void PrintQualifiedTeams(List<Group> groups)
        {
            foreach (var group in groups)
            {
                Console.WriteLine(group.Name);
                foreach (var team in group.Teams)
                {
                    Console.WriteLine(team.Name);
                }
                Console.WriteLine();
            }
        }

        public static void PrintTeams(List<Group> groups)
        {
            Console.WriteLine("ADVANCED TEAMS");
            foreach (var group in groups)
            {
                Console.WriteLine();
                foreach (var team in group.Teams)
                    Console.WriteLine(team.Name);
            }
        }

        public static void PrintTheMedals(List<Round> rounds)
        {
            Round final = rounds.First(x => x.RoundNumber == 7);
            Match finalMatch = final.Matches.First();
            Round thirdPlace = rounds.First(x => x.RoundNumber == 6);
            Match thirdPlaceMatch = thirdPlace.Matches.First();
            Team goldMedal = new Team();
            Team silverMedal = new Team();
            Team bronzeMedal = new Team();

            if (finalMatch.HomeScore > finalMatch.AwayScore)
            {
                goldMedal = finalMatch.HomeTeam;
                silverMedal = finalMatch.AwayTeam;
            }
            else
            {
                goldMedal = finalMatch.AwayTeam;
                silverMedal = finalMatch.HomeTeam;
            }
            if (thirdPlaceMatch.HomeScore > thirdPlaceMatch.AwayScore)
            {
                bronzeMedal = thirdPlaceMatch.HomeTeam;
            }
            else { bronzeMedal=thirdPlaceMatch.AwayTeam; }

            Console.WriteLine("GOLD MEDAL");
            Console.WriteLine(goldMedal.Name);
            Console.WriteLine();
            Console.WriteLine("SILVER MEDAL");
            Console.WriteLine(silverMedal.Name);
            Console.WriteLine();
            Console.WriteLine("BRONZE MEDAL");
            Console.WriteLine(bronzeMedal.Name);
        }
    }
}
