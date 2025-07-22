namespace Leagueinator.GUI.Model.Results.BowlsPlus {

    /// <summary>
    /// The results of a single team in a match.
    /// </summary>
    public class TeamResults : IComparable<TeamResults> {

        public Result Result { get; private set; }

        public TeamResults(MatchData matchData, int teamIndex) {
            this.Result = this.CalcResult(matchData);
            this.TeamIndex = teamIndex;
            this.Players = new(matchData.GetPlayers());
            this.Ends = matchData.Ends;
            this.Bowls = matchData.Score[teamIndex];
            this.Against = matchData.Score.Sum() - this.Bowls;
            this.TieBreaker = matchData.TieBreaker;
            this.Lane = matchData.Lane;
            this.Rank = -1; // Default rank, this is set by the DisplayRoundResults class
        }

        private Result CalcResult(MatchData matchData) {
            if (matchData.Score.Sum() == 0) return Result.Vacant;
            if (this.BowlsFor > this.BowlsAgainst) return Result.Win;
            if (this.BowlsFor < this.BowlsAgainst) return Result.Loss;
            if (this.PlusFor > this.PlusAgainst) return Result.Win;
            if (this.PlusFor < this.PlusAgainst) return Result.Loss;
            if (matchData.TieBreaker == this.TeamIndex) return Result.Win;
            if (matchData.TieBreaker != -1) return Result.Loss;
            return Result.Draw;
        }

        public override string ToString() {
            return $"{this.Rank}: {this.Result} [{this.Players}]  (For: {this.BowlsFor}+{this.PlusFor}, Against: {this.BowlsAgainst}+{this.PlusAgainst}, Ends: {this.Ends} Lane: {this.Lane + 1})";
        }

        public readonly int TieBreaker;

        public readonly int Lane;

        public readonly int Ends;

        public readonly int Bowls;

        public readonly int Against;

        public readonly Players Players;

        public readonly int TeamIndex;

        public int Rank { get; set; } = -1;

        public int BowlsFor => Math.Min((int)Math.Floor(this.Ends * 1.5), this.Bowls);

        public int BowlsAgainst => Math.Min((int)Math.Floor(this.Ends * 1.5), this.Against);

        public int PlusFor => this.Bowls - this.BowlsFor;

        public int PlusAgainst => this.Against - this.BowlsAgainst;

        public int CompareTo(TeamResults? other) {
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
