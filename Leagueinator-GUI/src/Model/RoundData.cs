using Leagueinator.GUI.Utility;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Text;

namespace Leagueinator.GUI.Model {

    public class RoundData : List<MatchData> {
        
        public RoundData() : base() {}

        public RoundData(EventData eventData){
            this.Fill(eventData);
        }

        public void Fill(EventData eventData) {
            while (this.Count < eventData.LaneCount) {
                this.Add(new MatchData(eventData.MatchFormat) {
                    Lane = this.Count,
                    Ends = eventData.DefaultEnds
                });
            }
        }

        public RoundData Copy() { 
            RoundData roundCopy = new();

            foreach (MatchData match in this) {
                MatchData matchCopy = new(match.MatchFormat) {
                    Lane = match.Lane,
                    Ends = match.Ends,
                    TieBreaker = match.TieBreaker
                };

                // Deep copy Teams
                for (int team = 0; team < match.Teams.Length; team++) {
                    matchCopy.Teams[team] = new string[match.Teams[team].Length];
                    for (int pos = 0; pos < match.Teams[team].Length; pos++) {
                        matchCopy.Teams[team][pos] = match.Teams[team][pos];
                    }
                }

                // Copy Score
                Array.Copy(match.Score, matchCopy.Score, match.Score.Length);

                roundCopy.Add(matchCopy);
            }

            return roundCopy;
        }

        public override string ToString() {
            StringBuilder sb = new();
            foreach (MatchData match in this) {
                sb.Append(match.ToString());
            }
            return sb.ToString();
        }

        public void RemovePlayer(string name) {
            if (string.IsNullOrEmpty(name)) {
                return;
            }

            foreach (MatchData match in this) {
                for (int team = 0; team < match.Teams.Length; team++) {
                    for (int position = 0; position < match.Teams[team].Length; position++) {
                        if (match.Teams[team][position] == name) {
                            match.Teams[team][position] = string.Empty;
                        }
                    }
                }
            }
        }

        public void SetPlayer(string name, int lane, int teamIndex, int position) {
            Debug.WriteLine($"SetPlayer: name={name}, lane={lane}, team={teamIndex}, pos={position}");
            if (this.HasPlayer(name)) {
                this.RemovePlayer(name);
            }

            if (teamIndex < 0 || teamIndex >= this.Count) {
                throw new ArgumentOutOfRangeException(nameof(teamIndex), "Team index is out of range.");
            }
            if (position < 0 || position >= this[teamIndex].Teams.Length) {
                throw new ArgumentOutOfRangeException(nameof(position), "Position is out of range.");
            }
            this[lane].Teams[teamIndex][position] = name;
        }

        public bool HasPlayer(string name) {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            foreach (MatchData match in this) {
                foreach (string[] team in match.Teams) {
                    if (team.Contains(name)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public (int, int, int) PollPlayer(string name) {
            foreach (MatchData match in this) {
                foreach (string[] team in match.Teams) {
                    if (team.Contains(name)) {
                        return (match.Lane, Array.IndexOf(match.Teams, team), Array.IndexOf(team, name));
                    }
                }
            }
            return (-1, -1, -1);
        }

        internal void AssignPlayersRandomly() {
            var dict = new Dictionary<(int, int, int), string>();

            // Populate the dictionary with player names and their positions
            foreach (MatchData match in this) {
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

                this[lane].Teams[team][position] = values.Pop();
            }
        }
    }
}
