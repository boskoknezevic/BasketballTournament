using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournament.Models
{
    public class Round
    {
        private int roundNumber;
        private List<Match> matches;

        public int RoundNumber { get => roundNumber; set => roundNumber = value; }
        public List<Match> Matches { get => matches; set => matches = value; }
    }
}
