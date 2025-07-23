using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    internal class AssignPlayers {
        public static void Balanced(LeagueData leagueData, RoundData roundData) {
            Dictionary<string, int> elo = ELO.CalculateELO(leagueData);

            List<string> namesSorted = roundData
                .SelectMany(matchData => matchData.Teams)
                .SelectMany(teamData => teamData.Players)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();


            namesSorted.Sort((name1, name2) => {
                return elo[name1].CompareTo(elo[name2]);
            });

            List<string> namesInterleaved = [];
            while (namesSorted.Count > 0) {
                namesInterleaved.Add(namesSorted.Pop());
                if (namesSorted.Count == 0) break;
                namesInterleaved.Add(namesSorted.Dequeue());
            }

            foreach (string name in namesInterleaved) {
                Debug.WriteLine($"{name} {elo[name]}");
            }

            roundData.ClearNames();

            while (namesInterleaved.Count > 0) {
                foreach (MatchData matchData in roundData) {
                    foreach (TeamData teamData in matchData.Teams) {
                        while (!teamData.IsFull() && namesInterleaved.Count > 0) {
                            string name = namesInterleaved.Pop();
                            Debug.WriteLine($"Add player {name} to Team");
                            teamData.AddPlayer(name);
                            Debug.WriteLine(teamData);
                            Debug.WriteLine($"{teamData.CountPlayers()} / {teamData.Names.Length}");
                        }
                    }
                }
            }
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
