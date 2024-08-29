using BasketballTournament.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasketballTournament.Methods
{
    public static class ProbabilityCalculator
    {
        public static void ReadProbabilityFromJSON(List<Group> groups)
        {
            // Prvi deo: Izračunavanje Form vrednosti na osnovu FibaRanking
            foreach (var group in groups)
            {
                foreach (var team in group.Teams)
                {
                    // Formula za izračunavanje Form na osnovu FibaRanking
                    team.Form = 0.3m + (0.6m - ((decimal)team.FibaRanking / 34.0m) * 0.6m);
                }
            }

            // Drugi deo: Čitanje exhibitions.json i ažuriranje Form vrednosti
            string exhibitionsPath = "C:\\Users\\bosko\\source\\repos\\BasketballTournament\\basketball-tournament-task-main\\exibitions.json";
            string exhibitionsJson = File.ReadAllText(exhibitionsPath);

            using (JsonDocument doc = JsonDocument.Parse(exhibitionsJson))
            {
                foreach (var group in groups)
                {
                    foreach (var team in group.Teams)
                    {
                        string isoCode = team.IsoCode;
                        if (doc.RootElement.TryGetProperty(isoCode, out JsonElement teamExhibitions))
                        {
                            foreach (JsonElement match in teamExhibitions.EnumerateArray())
                            {
                                string opponentCode = match.GetProperty("Opponent").GetString();
                                Team opponentTeam = groups.SelectMany(g => g.Teams).FirstOrDefault(t => t.IsoCode == opponentCode);
                                if (opponentTeam != null)
                                {
                                    string result = match.GetProperty("Result").GetString();
                                    int teamScore = int.Parse(result.Split('-')[0]);
                                    int opponentScore = int.Parse(result.Split('-')[1]);

                                    if (teamScore > opponentScore && team.FibaRanking > opponentTeam.FibaRanking)
                                    {
                                        // Povećaj Form za +0.05 ako je pobedio bolje rangiranog protivnika
                                        team.Form += 0.05m;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        public static void CalculateProbability(Match match)
        {
            if (match.HomeScore > match.AwayScore)
            {
                if (match.HomeTeam.FibaRanking > match.AwayTeam.FibaRanking)
                {
                    match.HomeTeam.Form += 0.05m;
                    match.AwayTeam.Form -= 0.05m;

                    if (match.HomeTeam.Form > 0.95m)
                    {
                        match.HomeTeam.Form = 0.95m;
                    }
                }
            }
            else
            {
                if (match.AwayTeam.FibaRanking > match.HomeTeam.FibaRanking)
                {
                    match.AwayTeam.Form += 0.05m;
                    match.HomeTeam.Form -= 0.05m;

                    if (match.AwayTeam.Form > 0.95m)
                    {
                        match.AwayTeam.Form = 0.95m;
                    }
                }
            }
        }
    }
}
