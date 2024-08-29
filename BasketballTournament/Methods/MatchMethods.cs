using BasketballTournament.Methods;
using BasketballTournament.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournament.Services
{
    public static class MatchMethods
    {
        public static List<Round> CreateASchedule(List<Group> groups)
        {
            List<Round> rounds = new List<Round>();
            int roundNumber = 1;

            var roundMatches = new Dictionary<int, List<Match>>();

            foreach (var group in groups)
            {
                List<Match> allMatches = GenerateAllPossibleGroupMatches(group);
                int numberOfRounds = allMatches.Count / (group.Teams.Count / 2);

                List<List<Match>> roundsList = Enumerable.Range(0, numberOfRounds)
                    .Select(_ => new List<Match>())
                    .ToList();

                foreach (var match in allMatches)
                {
                    bool matchAssigned = false;
                    for (int i = 0; i < numberOfRounds; i++)
                    {
                        if (roundsList[i].All(m => !m.HomeTeam.Equals(match.HomeTeam) && !m.AwayTeam.Equals(match.AwayTeam)) &&
                            !roundsList[i].Any(m => m.HomeTeam.Equals(match.AwayTeam) || m.AwayTeam.Equals(match.HomeTeam)))
                        {
                            roundsList[i].Add(match);
                            matchAssigned = true;
                            break;
                        }
                    }

                    if (!matchAssigned)
                    {
                        throw new InvalidOperationException("Could not assign match to any round.");
                    }
                }

                for (int i = 0; i < numberOfRounds; i++)
                {
                    if (!roundMatches.ContainsKey(i + 1))
                    {
                        roundMatches[i + 1] = new List<Match>();
                    }
                    roundMatches[i + 1].AddRange(roundsList[i]);
                }
            }

            foreach (var round in roundMatches)
            {
                rounds.Add(new Round
                {
                    RoundNumber = round.Key,
                    Matches = round.Value
                });
            }

            return rounds;
        }

        public static List<Match> GenerateAllPossibleGroupMatches(Group group)
        {
            List<Match> allMatches = new List<Match>();
            List<Team> teams = new List<Team>(group.Teams);

            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    allMatches.Add(new Match
                    {
                        HomeTeam = teams[i],
                        AwayTeam = teams[j]
                    });
                }
            }

            return allMatches;
        }

        public static Match SimulateMatch(Match match)
        {
            match.Date = DateTime.Now;
            Random random = new Random();

            // Verovatnoća predaje meča
            if (random.NextDouble() <= 0.05)
            {
                if (random.Next(0, 2) == 0)
                {
                    match.HomeScore = 30;
                    match.AwayScore = 0;
                }
                else
                {
                    match.HomeScore = 0;
                    match.AwayScore = 30;
                }
            }
            else
            {
                int homeBase = 60;
                int awayBase = 60;

                decimal formDifference = match.HomeTeam.Form - match.AwayTeam.Form;

                if (formDifference > 0)
                {
                    homeBase += (int)(formDifference * 20);
                }
                else
                {
                    awayBase += (int)(-formDifference * 20);
                }

                match.HomeScore = random.Next(homeBase, homeBase + 61);
                match.AwayScore = random.Next(awayBase, awayBase + 61);

                while (match.HomeScore == match.AwayScore)
                {
                    match.AwayScore = random.Next(awayBase, awayBase + 61);
                }
            }

            ProbabilityCalculator.CalculateProbability(match);

            match.HomeTeam.Matches.Add(match);
            match.AwayTeam.Matches.Add(match);
            return match;
        }


        public static List<Round> SimulateRound(List<Round> rounds)
        {
            foreach (var round in rounds)
                foreach (var match in round.Matches)
                {
                    SimulateMatch(match);
                }
                return rounds;
        }

        public static Dictionary<string, (int PointsScored, int PointsConceded, int Points)> CalculateGroupStatistics(Group group)
        {
            var teamStats = new Dictionary<string, (int PointsScored, int PointsConceded, int Points)>();

            foreach (var team in group.Teams)
            {
                var teamData = CalculateTeamStatistic(team);
                teamStats[team.Name] = teamData;
            }

            return teamStats;
        }

        public static (int PointsScored, int PointsConceded, int Points) CalculateTeamStatistic(Team team)
        {
            int pointsScored = 0;
            int pointsConceded = 0;
            int points = 0;

            foreach (var match in team.Matches)
            {
                if (match.HomeTeam.Name == team.Name)
                {
                    pointsScored += match.HomeScore;
                    pointsConceded += match.AwayScore;
                }
                else
                {
                    pointsScored += match.AwayScore;
                    pointsConceded += match.HomeScore;
                }

                if ((match.HomeTeam.Name == team.Name && match.HomeScore > match.AwayScore) ||
                    (match.AwayTeam.Name == team.Name && match.AwayScore > match.HomeScore))
                {
                    points += 2;
                }
                else if ((match.HomeScore == 30 && match.AwayScore == 0) || (match.HomeScore == 0 && match.AwayScore == 30))
                {
                    points += 0;
                }
                else 
                {
                    points += 1;
                }
            }

            return (pointsScored, pointsConceded, points);
        }


        public static Dictionary<char, Dictionary<string, (int PointsScored, int PointsConceded, int Points)>> CalculateAllGroupStatistics(List<Group> groups)
        {
            var allGroupStats = new Dictionary<char, Dictionary<string, (int PointsScored, int PointsConceded, int Points)>>();

            foreach (var group in groups)
            {
                var groupStats = CalculateGroupStatistics(group);
                var rankedGroup = RankTeamsInGroup(groupStats, group);
                allGroupStats.Add(group.Name, rankedGroup);
            }
            return allGroupStats;
        }


        public static Dictionary<string, (int PointsScored, int PointsConceded, int Points)> RankTeamsInGroup(Dictionary<string, (int PointsScored, int PointsConceded, int Points)> groupStats, Group group)
        {
            var rankedTeams = groupStats.OrderByDescending(t => t.Value.Points).ToList();
            var sortedDictionary = new Dictionary<string, (int PointsScored, int PointsConceded, int Points)>();

            var groupedTeams = rankedTeams.GroupBy(t => t.Value.Points).OrderByDescending(g => g.Key).ToList();

            foreach (var groupedTeam in groupedTeams)
            {
                var teamsWithSamePoints = groupedTeam.ToList();

                if (teamsWithSamePoints.Count > 1)
                {
                    var teamNames = teamsWithSamePoints.Select(t => t.Key).ToList();

                    var pointsDifferences = new Dictionary<string, int>();

                    foreach (var teamName in teamNames)
                    {
                        var team = group.Teams.FirstOrDefault(t => t.Name == teamName);
                        if (team != null)
                        {
                            int pointsDifference = 0;

                            foreach (var match in team.Matches)
                            {
                                if (match.HomeTeam.Name == teamName || match.AwayTeam.Name == teamName)
                                {
                                    var opponentName = match.HomeTeam.Name == teamName ? match.AwayTeam.Name : match.HomeTeam.Name;

                                    if (teamNames.Contains(opponentName))
                                    {
                                        pointsDifference += (match.HomeTeam.Name == teamName ? match.HomeScore - match.AwayScore : match.AwayScore - match.HomeScore);
                                    }
                                }
                            }

                            pointsDifferences[teamName] = pointsDifference;
                        }
                    }

                    var rankedTiedTeams = teamsWithSamePoints
                        .OrderByDescending(t => pointsDifferences.ContainsKey(t.Key) ? pointsDifferences[t.Key] : 0)
                        .ThenBy(t => t.Key)
                        .ToList();

                    foreach (var team in rankedTiedTeams)
                    {
                        sortedDictionary[team.Key] = team.Value;
                    }
                }
                else
                {
                    foreach (var team in teamsWithSamePoints)
                    {
                        sortedDictionary[team.Key] = team.Value;
                    }
                }
            }

            group.Teams = group.Teams
                                .OrderBy(t => Array.IndexOf(sortedDictionary.Keys.ToArray(), t.Name))
                                .ToList();

            return sortedDictionary;
        }


        public static void AdvanceFromGroups(List<Group> groups, Dictionary<char, Dictionary<string, (int PointsScored, int PointsConceded, int Points)>> allGroupsStats)
        {
            foreach (var group in groups)
            {
                var teamToRemove = group.Teams.Last().Name; // Pretpostavlja se da `Name` identifikuje tim

                // Ukloni tim iz grupe
                group.Teams.RemoveAt(group.Teams.Count - 1);

                // Ako grupa postoji u `allGroupsStats`, ukloni statistiku za taj tim
                if (allGroupsStats.TryGetValue(group.Name, out var groupStats))
                {
                    groupStats.Remove(teamToRemove);
                }
            }
        }

        public static List<Group> CalculateTop8(List<Group> groups, Dictionary<char, Dictionary<string, (int PointsScored, int PointsConceded, int Points)>> allGroupsStats)
        {
            var newGroups = new List<Group>();
            var newAllGroupsStats = new Dictionary<char, Dictionary<string, (int PointsScored, int PointsConceded, int Points)>>();

            // Pretvori svaku grupu u red
            var teamQueues = groups.Select(g => new Queue<Team>(g.Teams)).ToList();
            var statsQueues = allGroupsStats.Select(kvp => new Queue<KeyValuePair<string, (int PointsScored, int PointsConceded, int Points)>>(kvp.Value)).ToList();

            int groupIndex = 0;

            // Prolazi kroz sve grupe dokle god ima timova u bilo kojoj grupi
            while (teamQueues.Any(q => q.Any()))
            {
                var newGroup = new Group { Teams = new List<Team>() };
                var newGroupStats = new Dictionary<string, (int PointsScored, int PointsConceded, int Points)>();

                // Uzmi prvog člana iz svake grupe
                for (int i = 0; i < teamQueues.Count; i++)
                {
                    if (teamQueues[i].Any())
                    {
                        var team = teamQueues[i].Dequeue();
                        newGroup.Teams.Add(team);

                        if (statsQueues[i].Any())
                        {
                            var teamStat = statsQueues[i].Dequeue();
                            newGroupStats[teamStat.Key] = teamStat.Value;
                        }
                    }
                }

                if (newGroup.Teams.Any())
                {
                    newGroups.Add(newGroup);
                    newAllGroupsStats[(char)('A' + groupIndex)] = newGroupStats;
                    groupIndex++;
                }
            }

            RankTeamsInKnockout(newGroups, newAllGroupsStats);
            return newGroups;
        }


        public static void RankTeamsInKnockout(List<Group> newGroups, Dictionary<char, Dictionary<string, (int PointsScored, int PointsConceded, int Points)>> allGroupsStats)
        {
            // Iterate over each group in newGroups
            foreach (var group in newGroups)
            {
                // Rank teams within the group
                var rankedTeams = group.Teams
                    .Select(team => new
                    {
                        Team = team,
                        Stats = allGroupsStats.Values
                            .SelectMany(dict => dict)
                            .FirstOrDefault(pair => pair.Key == team.Name).Value
                    })
                    .OrderByDescending(x => x.Stats.Points)
                    .ThenByDescending(x => x.Stats.PointsScored - x.Stats.PointsConceded)
                    .ThenByDescending(x => x.Stats.PointsScored)
                    .ToList();

                // Update the group with ranked teams
                group.Teams = rankedTeams.Select(x => x.Team).ToList();
            }

            // Remove the last team from the last group
            var lastGroup = newGroups.Last();
            if (lastGroup.Teams.Any())
            {
                lastGroup.Teams.RemoveAt(lastGroup.Teams.Count - 1);
            }
        }


        public static List<Group> CreateKnockoutGroups(List<Group> groups)
        {
            List<Team> teams = new List<Team>();
            foreach (var group in groups)
            {
                foreach (var team in group.Teams)
                {
                    teams.Add(team);
                }
            }
            List<Group> pots = new List<Group>();
            var groupNames = new[] { 'D', 'E', 'F', 'G'}; // Nazivi grupa
            int teamsPerGroup = 2;

            for (int i = 0; i < groupNames.Length; i++)
            {
                var group = new Group
                {
                    Name = groupNames[i],
                    Teams = new List<Team>()
                };

                // Dodaj timove u grupu
                for (int j = 0; j < teamsPerGroup; j++)
                {
                    group.Teams.Add(teams[i * teamsPerGroup + j]);
                }

                pots.Add(group);
            }
            return pots;
        }
        public static List<Round> CreateQuarterFinals(List<Group> pots)
        {
            List<Round> quarterFinalRound = new List<Round>();
            List<Match> quarterFinalMatchesDandG = new List<Match>();
            List<Match> quarterFinalMatchesEandF = new List<Match>();

            var matches = pots.SelectMany(g => g.Teams)
                .SelectMany(t => t.Matches)
                .ToList();

            bool HavePlayed(Team team1, Team team2)
            {
                return matches.Any(m =>
                    (m.HomeTeam == team1 && m.AwayTeam == team2) ||
                    (m.HomeTeam == team2 && m.AwayTeam == team1));
            }

            Group potD = pots.FirstOrDefault(g => g.Name == 'D');
            Group potE = pots.FirstOrDefault(g => g.Name == 'E');
            Group potF = pots.FirstOrDefault(g => g.Name == 'F');
            Group potG = pots.FirstOrDefault(g => g.Name == 'G');

            while (quarterFinalMatchesDandG.Count < 2)
            {
                quarterFinalMatchesDandG.Clear();
                foreach (var team in potD.Teams)
                {
                    var random = new Random();
                    var randomTeam = potG.Teams.OrderBy(x => random.Next()).FirstOrDefault();
                    if (randomTeam == null) { break; }
                    if (HavePlayed(team, randomTeam)) { break; }
                    var matchDandG = new Match
                    {
                        HomeTeam = team,
                        AwayTeam = randomTeam,
                    };
                    quarterFinalMatchesDandG.Add(matchDandG);
                    potG.Teams.Remove(randomTeam);
                }
            }

            while (quarterFinalMatchesEandF.Count < 2)
            {
                quarterFinalMatchesEandF.Clear();
                foreach (var team in potE.Teams)
                {
                    var random = new Random();
                    var randomTeam = potF.Teams.OrderBy(x => random.Next()).FirstOrDefault();
                    if(randomTeam == null) { break; }
                    if (HavePlayed(team, randomTeam)) { break; }
                    var matchEandF = new Match
                    {
                        HomeTeam = team,
                        AwayTeam = randomTeam,
                    };
                    quarterFinalMatchesEandF.Add(matchEandF);
                    potF.Teams.Remove(randomTeam);
                }
            }
            Round quarterFinalDandG = new Round { RoundNumber = 4, Matches = quarterFinalMatchesDandG };
            Round quarterFinalEandF= new Round { RoundNumber = 4, Matches = quarterFinalMatchesEandF };
            quarterFinalRound.Add(quarterFinalDandG);
            quarterFinalRound.Add(quarterFinalEandF);
            return quarterFinalRound;
        }
        
        public static List<Round> CreateSemiFinals(List<Round> quarterFinals)
        {
            List<Team> winners = new List<Team>();
            List<Round> semiFinals = new List<Round>();
            List<Match> semiFinalMatches = new List<Match>();
            Random random = new Random();
            foreach (var quarterFinal in quarterFinals)
            {
                foreach (var match in quarterFinal.Matches)
                {
                    if (match.HomeScore > match.AwayScore) { winners.Add(match.HomeTeam); }
                    else { winners.Add(match.AwayTeam); }
                }
                var homeTeam = winners.OrderBy(x => random.Next()).First();
                var awayTeam = winners.First(x => x != homeTeam);

                semiFinalMatches.Add(new Match { HomeTeam = homeTeam, AwayTeam = awayTeam });
                winners.Clear();
            }
            semiFinals.Add(new Round { RoundNumber = 5, Matches = semiFinalMatches });
            return semiFinals;
        }
    
        public static List<Round> CreateFinalsAndThirdPlaceGame(List<Round> semiFinals)
        {
            List<Team> winners = new List<Team>();
            List<Team> thirdPlace = new List<Team>();
            Match finalMatch = new Match();
            Match thirdPlaceMatch = new Match();
            Random random = new Random();

            List<Match> final = new List<Match>();
            Round finalRound = new Round();
            List<Match> thirdPlaced = new List<Match>();
            Round thirdPlaceRound = new Round();


            foreach (var quarterFinal in semiFinals)
            {
                foreach (var match in quarterFinal.Matches)
                {
                    if (match.HomeScore > match.AwayScore)
                    {
                        winners.Add(match.HomeTeam);
                        thirdPlace.Add(match.AwayTeam);
                    }
                    else
                    {
                        winners.Add(match.AwayTeam);
                        thirdPlace.Add(match.HomeTeam);
                    }
                }
                var homeTeamFinals = winners.OrderBy(x => random.Next()).First();
                var awayTeamFinals = winners.First(x => x != homeTeamFinals);

                finalMatch.HomeTeam = homeTeamFinals;
                finalMatch.AwayTeam = awayTeamFinals;
                winners.Clear();
                final.Add(finalMatch);
                finalRound = new Round { RoundNumber = 7, Matches = final };

                var homeTeamThirdPlace = thirdPlace.OrderBy(x => random.Next()).First();
                var awayTeamThirdPlace = thirdPlace.First(x => x != homeTeamThirdPlace);

                thirdPlaceMatch.HomeTeam = homeTeamThirdPlace;
                thirdPlaceMatch.AwayTeam = awayTeamThirdPlace;
                thirdPlace.Clear();
                thirdPlaced.Add(thirdPlaceMatch);
                thirdPlaceRound = new Round { RoundNumber = 6, Matches = thirdPlaced };
            }
            List<Round> finalRounds = [thirdPlaceRound, finalRound];

            return finalRounds;
        }
    
    
    }









}

