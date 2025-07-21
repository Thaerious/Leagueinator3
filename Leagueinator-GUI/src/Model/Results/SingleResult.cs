using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Model.Results {

    public class SingleResult : IComparable<SingleResult> {

        public SingleResult(MatchData matchData, int teamIndex) {
            this.MatchData = matchData;
            this.TeamIndex = teamIndex;
            this.Ends = matchData.Ends;
            this.Bowls = matchData.Score[teamIndex];
            this.Against = matchData.Score.Sum() - this.Bowls;
            this.Rank = -1; // Default rank, this is set by the DisplayRoundResults class
            this.Players = [..matchData.Teams[teamIndex].Players];
        }

        public Players Players { get; }

        public Result Result {
            get {
                if (this.MatchData.Score.Sum() == 0) return Result.Vacant;
                if (this.BowlsFor > this.BowlsAgainst) return Result.Win;
                if (this.BowlsFor < this.BowlsAgainst) return Result.Loss;
                if (this.PlusFor > this.PlusAgainst) return Result.Win;
                if (this.PlusFor < this.PlusAgainst) return Result.Loss;
                if (this.MatchData.TieBreaker == this.TeamIndex) return Result.Win;
                if (this.MatchData.TieBreaker != -1) return Result.Loss;
                return Result.Draw;
            }
        }

        public override string ToString() {
            return $"{this.Rank}: {this.Result} [{this.Players}]  (For: {this.BowlsFor}+{this.PlusFor}, Against: {this.BowlsAgainst}+{this.PlusAgainst}, Ends: {this.Ends} Lane: {this.MatchData.Lane + 1})";
        }

        public int Ends { get; set; } = 0;

        public int Bowls { get; set; } = 0;

        public int Against { get; set; } = 0;

        public int Rank { get; set; } = -1;

        public int BowlsFor {
            get => Math.Min((int)Math.Floor(this.Ends * 1.5), this.Bowls);
        }

        public int BowlsAgainst {
            get => Math.Min((int)Math.Floor(this.Ends * 1.5), this.Against);
        }

        public int PlusFor {
            get => this.Bowls - this.BowlsFor;
        }

        public int PlusAgainst {
            get => this.Against - this.BowlsAgainst;
        }
        public MatchData MatchData { get; }

        public readonly int TeamIndex;

        public int CompareTo(SingleResult? other) {
            if (other == null) return 1; // Null is less

            int cmp = this.Result.CompareTo(other.Result);
            if (cmp != 0) return -cmp; // Higher Result wins (Win > Draw > Loss)

            cmp = this.BowlsFor.CompareTo(other.BowlsFor); // Higher is better
            if (cmp != 0) return cmp;

            cmp = this.PlusFor.CompareTo(other.PlusFor); // Higher is better
            if (cmp != 0) return cmp;

            cmp = other.BowlsAgainst.CompareTo(this.BowlsAgainst); // Lower is better
            if (cmp != 0) return cmp;

            cmp = other.PlusAgainst.CompareTo(this.PlusAgainst); // Lower is better
            if (cmp != 0) return cmp;

            return 0; // Equal results
        }
    }
}
