using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournament.Models
{
    public class Team
    {
        private string name;
        private string isoCode;
        private int fibaRanking;
        private decimal form;
        private List<Match> matches = new List<Match>();

        public string Name { get => name; set => name = value; }
        public string IsoCode { get => isoCode; set => isoCode = value; }
        public int FibaRanking { get => fibaRanking; set => fibaRanking = value; }
        public decimal Form { get => form; set => form = value; }
        public List<Match> Matches { get => matches; set => matches = value; }
    }
}
