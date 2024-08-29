using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournament.Models
{
    public class Group
    {
        private char name;
        private List<Team> teams = new List<Team>();

        public char Name { get => name; set => name = value; }
        public List<Team> Teams { get => teams; set => teams = value; }
    }
}
