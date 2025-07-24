using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    internal class AssignPlayers {
        public static void Balanced(LeagueData leagueData, EventData eventData, RoundData roundData) {
            Dictionary<string, int> elo = ELO.CalculateELO(leagueData);

            List<string> namesSorted = roundData
                .SelectMany(matchData => matchData.Teams)
                .SelectMany(teamData => teamData.Players)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();

            namesSorted.Sort((name1, name2) => {
                return elo[name2].CompareTo(elo[name1]);
            });

            roundData.ClearNames();

            foreach (TeamData teamData in roundData.Teams) {
                if (namesSorted.Count == 0) break;
                Debug.WriteLine(namesSorted.JoinString());

                string name = namesSorted.Dequeue();
                teamData.AddPlayer(name);
                Debug.WriteLine($" + {name} : {eventData.Stats.PreviousPartners(name).JoinString()}");

                if (namesSorted.Count == 0) break;

                while (!teamData.IsFull()) {
                    string partner = NextPartner(name, namesSorted, eventData);
                    teamData.AddPlayer(partner);
                    Debug.WriteLine($" + {partner}");
                }
            }
        }

        private static string NextPartner(string name, List<string> sorted, EventData eventData) {
            for (int i = sorted.Count - 1; i >= 0; i--) {
                var previous = eventData.Stats.PreviousPartners(name);
                if (!previous.Contains(sorted[i])) {
                    var partner = sorted[i];
                    sorted.RemoveAt(i);
                    return partner;
                }
            }

            throw new Exception("No valid partner found");
        }

        public static void Randomly(LeagueData leagueData, RoundData roundData) {
            var dict = new Dictionary<(int, int, int), string>();

            // Populate the dictionary with player Names and their positions
            foreach (MatchData match in roundData) {
                for (int team = 0; team < match.Teams.Length; team++) {
                    for (int position = 0; position < match.Teams[team].Length; position++) {
                        if (!string.IsNullOrEmpty(match.Teams[team][position])) {
                            dict[(match.Lane, team, position)] = match.Teams[team][position];
                        }
                    }
                }
            }

            // Extract keys and values
            var keys = dict.Keys.ToList();
            var values = dict.Values.ToList();

            // Shuffle values
            var rng = new Random();
            values = values.OrderBy(_ => rng.Next()).ToList();

            // Reassign shuffled values back to their original positions
            foreach (var key in keys) {
                var lane = key.Item1;
                var team = key.Item2;
                var position = key.Item3;

                roundData[lane].Teams[team][position] = values.Pop();
            }
        }
    }
}
