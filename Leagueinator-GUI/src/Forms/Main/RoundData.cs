using Leagueinator.GUI.Utility;
using System.Diagnostics;
using System.Text;

namespace Leagueinator.GUI.Forms.Main {

    public class PlayerData(string Name, int Team) {
        public string Name { get; set; } = Name.Trim();
        public int Team { get; set; } = Team;
        //public bool IsReady { get; set; } = false;
        //public bool IsCaptain { get; set; } = false;
        //public bool IsSubstitute { get; set; } = false;
    }

    public class MatchData {
        private MatchFormat _matchFormat = MatchFormat.VS2;

        public MatchFormat MatchFormat { 
            get => this._matchFormat;
            set {
                this._matchFormat = value;
                this.Score = new int[MatchFormatMeta.Info[this.MatchFormat].TeamCount];
            }
        }
        public int Lane { get; set; } = -1;
        public int Ends { get; set; } = 0;
        public int[] Score { get; private set; } = [];
        public readonly List<PlayerData> Players = [];
        public int TieBreaker { get; set; } = -1;

        public MatchData(MatchFormat matchFormat) {
            this.MatchFormat = matchFormat;
            this.Score = new int[MatchFormatMeta.Info[this.MatchFormat].TeamCount];
        }

        public void Replace(string oldName, string newName) {
            for (int i = 0; i < this.Players.Count; i++) {
                if (this.Players[i].Name == oldName) {
                    this.Players[i].Name = newName.Trim();
                }
            }
        }

        public override string ToString() {
            StringBuilder sb = new();
            sb.Append($"Match {this.Lane}: {this.MatchFormat} | Players: ");
            foreach (PlayerData player in this.Players) {
                sb.Append($"{player.Name} (Team {player.Team}), ");
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

            for (int i = 0; i < this.Count; i++) {
                MatchData match = this[i];
                MatchData matchCopy = new(match.MatchFormat) {
                    Lane = match.Lane,
                    TieBreaker = match.TieBreaker
                };
                matchCopy.Players.AddRange(match.Players.Select(p => new PlayerData(p.Name, p.Team)));
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
    }
}
