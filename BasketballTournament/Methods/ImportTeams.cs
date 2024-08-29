using BasketballTournament.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasketballTournament.Methods
{
    public static class ImportTeams
    {
        public static List<Group> ImportTeamsFromJSON()
        {
            List<Group> groups = new List<Group>();
            string filePath = "C:\\Users\\bosko\\source\\repos\\BasketballTournament\\basketball-tournament-task-main\\groups.json";

            string jsonString = File.ReadAllText(filePath);

            using (JsonDocument doc = JsonDocument.Parse(jsonString))
            {
                foreach (JsonProperty group in doc.RootElement.EnumerateObject())
                {
                    Group newGroup = new Group
                    {
                        Name = Convert.ToChar(group.Name)
                    };

                    foreach (JsonElement teamElement in group.Value.EnumerateArray())
                    {
                        Team newTeam = new Team
                        {
                            Name = teamElement.GetProperty("Team").GetString(),
                            IsoCode = teamElement.GetProperty("ISOCode").GetString(),
                            FibaRanking = teamElement.GetProperty("FIBARanking").GetInt32()
                        };
                        newGroup.Teams.Add(newTeam);
                    }
                    groups.Add(newGroup);
                }
            }
            return groups;
        }
    }
}
