using Leagueinator.GUI.Utility;
using System.Diagnostics;
using System.Text;

namespace Leagueinator.GUI.Forms.Main {
    public class MatchData {
        private MatchFormat _matchFormat = MatchFormat.VS2;

        public MatchFormat MatchFormat { 
            get => this._matchFormat;
            set {
                int from = MatchFormatMeta.Info[this._matchFormat].TeamSize;
                int to = MatchFormatMeta.Info[value].TeamSize;

                for (int i = from; i < to; i++) {
                    
                }

                this._matchFormat = value;
                this.Score = new int[MatchFormatMeta.Info[this.MatchFormat].TeamCount];
            }
        }
        public int Lane { get; set; } = -1;
        public int Ends { get; set; } = 0;
        public int[] Score { get; private set; } = [];

        public readonly string[][] Players = [];
        public int TieBreaker { get; set; } = -1;

        public MatchData(MatchFormat matchFormat) {
            var teamCount = MatchFormatMeta.Info[matchFormat].TeamCount;
            var teamSize = MatchFormatMeta.Info[matchFormat].TeamSize;

            this.MatchFormat = matchFormat;
            this.Score = new int[teamCount];
            this.Players = new string[teamCount][];

            for (int i = 0; i < this.Score.Length; i++) {
                this.Score[i] = 0;
            }

            for (int i = 0; i < teamCount; i++) {
                this.Players[i] = new string[teamSize];
                for (int j = 0; j < teamSize; j++) {
                    this.Players[i][j] = new string(string.Empty);
                }
            }
        }

        public int CountPlayers() {
            int count = 0;
            foreach (string[] team in this.Players) {
                foreach (string player in team) {
                    if (!string.IsNullOrEmpty(player)) {
                        count++;
                    }
                }
            }
            return count;
        }

        public override string ToString() {
            StringBuilder sb = new();
            sb.Append($"Match {this.Lane}: {this.MatchFormat} | Players: ");
            foreach (string[] team in this.Players) {
                sb.Append($"[");
                foreach (string player in team) {
                    if (!string.IsNullOrEmpty(player)) {
                        sb.Append($"{player}, ");
                    }
                }
                sb.Append("], ");
            }

            sb.Append($"Score: {string.Join(", ", this.Score)} | TB: {this.TieBreaker} | Ends: {this.Ends}\n");
            return sb.ToString();
        }
    }

    public class RoundData : List<MatchData> {
        
        private RoundData() : base() {}

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

        public void SetPlayer(string name, int lane, int teamIndex, int position) {
            if (this.HasPlayer(name)) {
                throw new ArgumentException($"Player {name} already exists in the match data.", nameof(name));
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
    }
}
