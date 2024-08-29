using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournament.Models
{
    public class Match
    {
        private Team homeTeam;
        private Team awayTeam;
        private int homeScore;
        private int awayScore;
        private DateTime date;

        public Team HomeTeam { get => homeTeam; set => homeTeam = value; }
        public Team AwayTeam { get => awayTeam; set => awayTeam = value; }
        public int HomeScore { get => homeScore; set => homeScore = value; }
        public int AwayScore { get => awayScore; set => awayScore = value; }
        public DateTime Date { get => date; set => date = value; }


        public override bool Equals(object obj)
        {
            if (obj is Match other)
            {
                return (HomeTeam == other.HomeTeam && AwayTeam == other.AwayTeam) ||
                       (HomeTeam == other.AwayTeam && AwayTeam == other.HomeTeam);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (HomeTeam?.GetHashCode() ?? 0);
                hash = hash * 23 + (AwayTeam?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
