using Leagueinator.GUI.Model.Enums;

namespace Leagueinator.GUI.Model.Results.BowlsPlus {

    /// <summary>
    /// The results of a single team in a match.
    /// </summary>
    public class TeamResults : IComparable<TeamResults> {

        public GameResult Result { get; private set; }

        public TeamResults(MatchData matchData, int teamIndex) {
            this.Result = this.CalcResult(matchData);
            this.TeamIndex = teamIndex;
            this.Players = matchData.AllNames();
            this.Ends = matchData.Ends;
            this.Bowls = matchData.Score[teamIndex];
            this.Against = matchData.Score.Sum() - this.Bowls;
            this.TieBreaker = matchData.TieBreaker;
            this.Lane = matchData.Lane;
            this.Rank = -1; // Default rank, this is set by the DisplayRoundResults class
        }

        private GameResult CalcResult(MatchData matchData) {
            if (matchData.Score.Sum() == 0) return GameResult.Vacant;
            if (this.BowlsFor > this.BowlsAgainst) return GameResult.Win;
            if (this.BowlsFor < this.BowlsAgainst) return GameResult.Loss;
            if (this.PlusFor > this.PlusAgainst) return GameResult.Win;
            if (this.PlusFor < this.PlusAgainst) return GameResult.Loss;
            if (matchData.TieBreaker == this.TeamIndex) return GameResult.Win;
            if (matchData.TieBreaker != -1) return GameResult.Loss;
            return GameResult.Tie;
        }

        public override string ToString() {
            return $"{this.Rank}: {this.Result} [{this.Players}]  (For: {this.BowlsFor}+{this.PlusFor}, Against: {this.BowlsAgainst}+{this.PlusAgainst}, Ends: {this.Ends} Lane: {this.Lane + 1})";
        }

        public readonly int TieBreaker;

        public readonly int Lane;

        public readonly int Ends;

        public readonly int Bowls;

        public readonly int Against;

        public readonly List<string> Players;

        public readonly int TeamIndex;

        public int Rank { get; set; } = -1;

        public int BowlsFor => Math.Min((int)Math.Floor(this.Ends * 1.5), this.Bowls);

        public int BowlsAgainst => Math.Min((int)Math.Floor(this.Ends * 1.5), this.Against);

        public int PlusFor => this.Bowls - this.BowlsFor;

        public int PlusAgainst => this.Against - this.BowlsAgainst;

        public int CompareTo(TeamResults? other) {
            if (other == null) return 1; // Null is less

            int cmp = this.Result.CompareTo(other.Result);
            if (cmp != 0) return -cmp; // Higher GameResult wins (Win > Tie > Loss)

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
