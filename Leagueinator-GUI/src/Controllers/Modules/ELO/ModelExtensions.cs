using Leagueinator.GUI.Model;
using System.Diagnostics;
using Utility;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.ELO {
    public static class ModelExtensions {

        public static int Fitness(this RoundData roundData) {
            int fitness = 0;
            ELODictionary ELO = new (roundData);
            var leagueCounts = roundData.Parent.Parent.CountTimesPartnered();
            var eventCounts = roundData.Parent.CountTimesPartnered();
            DefaultDictionary<Players, int> fitnessByTeam = new(0);

            foreach (TeamData teamData in roundData.AllTeams().Where(t => !t.IsEmpty())) {
                double elo = ELO.GetELO(teamData.ToPlayers()) / teamData.ToPlayers().Count;
                double countInLeague = leagueCounts[teamData.ToPlayers()] - 1;
                double countInEvent = eventCounts[teamData.ToPlayers()];
                double leagueMultiplier = 1 + (countInLeague / 4);
                double eventMultiplier = 1 + (countInEvent * 10);
                fitnessByTeam[teamData.ToPlayers()] = (int)(elo * leagueMultiplier * eventMultiplier);

                Debug.WriteLine($"{teamData.ToPlayers().JoinString()} = {elo} * {leagueMultiplier} * {eventMultiplier} = {fitnessByTeam[teamData.ToPlayers()]}");               
            }

            foreach (MatchData matchData in roundData.Matches) {
                if (matchData.AllNames().Count == 0) continue;
                foreach (TeamData teamData1 in matchData.Teams) {
                    foreach (TeamData teamData2 in matchData.Teams) {
                        int elo1 = fitnessByTeam[teamData1.ToPlayers()];
                        int elo2 = fitnessByTeam[teamData2.ToPlayers()];

                        int diff = elo1 - elo2;
                        fitness += diff * diff;
                    }
                }
            }
            return (int)Math.Sqrt(fitness);
        }

        public static DefaultDictionary<Players, int> CountTimesPartnered(this LeagueData leagueData) {
            DefaultDictionary<Players, int> counts = new(0);
            foreach (TeamData teamData in leagueData.AllTeams()) {
                counts[teamData.ToPlayers()] += 1;
            }
            return counts;
        }

        public static DefaultDictionary<Players, int> CountTimesPartnered(this EventData eventData) {
            DefaultDictionary<Players, int> counts = new(0);
            foreach (TeamData teamData in eventData.AllTeams()) {
                counts[teamData.ToPlayers()] += 1;
            }
            return counts;
        }
    }
}
