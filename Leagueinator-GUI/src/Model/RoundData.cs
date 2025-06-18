using Leagueinator.GUI.Utility;
using System.Text;

namespace Leagueinator.GUI.Model {

    public class RoundData : List<MatchData> {
        
        public RoundData() : base() {}

        public RoundData(EventData eventData) : this(eventData.MatchFormat, eventData.LaneCount, eventData.DefaultEnds) {}

        public RoundData(MatchFormat matchFormat, int LaneCount, int DefaultEnds) {
            while (this.Count < LaneCount) {
                this.Add(new MatchData(matchFormat) {
                    Lane = this.Count,
                    Ends = DefaultEnds
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

                // Deep copy Players
                for (int team = 0; team < match.Players.Length; team++) {
                    matchCopy.Players[team] = new string[match.Players[team].Length];
                    for (int pos = 0; pos < match.Players[team].Length; pos++) {
                        matchCopy.Players[team][pos] = match.Players[team][pos];
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
                for (int team = 0; team < match.Players.Length; team++) {
                    for (int position = 0; position < match.Players[team].Length; position++) {
                        if (match.Players[team][position] == name) {
                            match.Players[team][position] = string.Empty;
                        }
                    }
                }
            }
        }

        public void SetPlayer(string name, int lane, int teamIndex, int position) {
            if (this.HasPlayer(name)) {
                this.RemovePlayer(name);
            }

            if (teamIndex < 0 || teamIndex >= this.Count) {
                throw new ArgumentOutOfRangeException(nameof(teamIndex), "Team index is out of range.");
            }
            if (position < 0 || position >= this[teamIndex].Players.Length) {
                throw new ArgumentOutOfRangeException(nameof(position), "Position is out of range.");
            }
            this[lane].Players[teamIndex][position] = name;
        }

        public bool HasPlayer(string name) {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            foreach (MatchData match in this) {
                foreach (string[] team in match.Players) {
                    if (team.Contains(name)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public (int, int, int) PollPlayer(string name) {
            foreach (MatchData match in this) {
                foreach (string[] team in match.Players) {
                    if (team.Contains(name)) {
                        return (match.Lane, Array.IndexOf(match.Players, team), Array.IndexOf(team, name));
                    }
                }
            }
            return (-1, -1, -1);
        }
    }
}
