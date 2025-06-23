using Leagueinator.GUI.Model.Results;
using System.Text;

namespace Leagueinator.GUI.Model {
    public class MatchData {
        private MatchFormat _matchFormat = MatchFormat.VS2;

        public MatchFormat MatchFormat { 
            get => this._matchFormat;
            set {
                var teamCount = MatchFormatMeta.Info[value].TeamCount;
                var teamSize = MatchFormatMeta.Info[value].TeamSize;

                this._matchFormat = value;
                this.Score = new int[teamCount];
                var OldPlayers = this.Teams;
                this.Teams = new string[teamCount][];               

                for (int i = 0; i < this.Score.Length; i++) {
                    this.Score[i] = 0;
                }

                for (int i = 0; i < teamCount; i++) {
                    this.Teams[i] = new string[teamSize];
                    Array.Fill(this.Teams[i], string.Empty);
                    // TODO Retain old players if possible
                }
            }
        }
        public int Lane { get; set; } = -1;
        public int Ends { get; set; } = 0;
        public int[] Score { get; set; } = [];

        public string[][] Teams { get; set; } = [];
        public int TieBreaker { get; set; } = -1;

        public MatchData(MatchFormat matchFormat) {
            var teamCount = MatchFormatMeta.Info[matchFormat].TeamCount;
            var teamSize = MatchFormatMeta.Info[matchFormat].TeamSize;

            this.MatchFormat = matchFormat;
            this.Score = new int[teamCount];
            this.Teams = new string[teamCount][];

            for (int i = 0; i < this.Score.Length; i++) {
                this.Score[i] = 0;
            }

            for (int i = 0; i < teamCount; i++) {
                this.Teams[i] = new string[teamSize];
                Array.Fill(this.Teams[i], string.Empty);
            }
        }

        public int CountPlayers() {
            int count = 0;
            foreach (string[] team in this.Teams) {
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

            foreach (string[] team in this.Teams) {
                sb.Append($"[");
                sb.Append(string.Join(", ", team));
                sb.Append("], ");
            }

            sb.Append($"Score: {string.Join(", ", this.Score)} | TB: {this.TieBreaker} | Ends: {this.Ends}\n");
            return sb.ToString();
        }

        public bool HasBowls() {
            if (this.Score.Sum() == 0) {
                return false;
            }
            return true;
        }
    }
}
