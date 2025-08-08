using Leagueinator.GUI.Model.Enums;
using Utility.Extensions;

namespace Leagueinator.GUI.Model.Results.BowlsPlus {

    public class SingleResult : IComparable<SingleResult> {

        public SingleResult(MatchData matchData, int teamIndex) {
            this.MatchData = matchData;
            this.TeamIndex = teamIndex;
            this.Ends = matchData.Ends;
            this.Rank = -1; // Default rank, this is set by the DisplayRoundResults class
            this.Players = [.. matchData.Teams[teamIndex].Names.Where(p => !string.IsNullOrEmpty(p))];
            this.MatchScore = [.. matchData.Score];
        }

        private readonly int[] MatchScore;

        public Players Players { get; }

        public GameResult Result {
            get {
                if (this.MatchData.Score.Sum() == 0) return GameResult.Vacant;
                if (this.BowlsFor > this.BowlsAgainst) return GameResult.Win;
                if (this.BowlsFor < this.BowlsAgainst) return GameResult.Loss;
                if (this.PlusFor > this.PlusAgainst) return GameResult.Win;
                if (this.PlusFor < this.PlusAgainst) return GameResult.Loss;
                if (this.MatchData.TieBreaker == this.TeamIndex) return GameResult.Win;
                if (this.MatchData.TieBreaker != -1) return GameResult.Loss;
                return GameResult.Draw;
            }
        }

        private int Limit {
            get {
                if (this.MatchData.MatchFormat == MatchFormat.A4321) {
                    return (int)(Math.Floor(this.Ends * 7.5));
                }
                else {
                    return (int)(Math.Floor(this.Ends / 1.5));
                }
            }
        }

        public override string ToString() {
            return $"{this.Rank}: {this.Result} [{this.Players.JoinString()}]  (For: {this.BowlsFor}+{this.PlusFor}, Against: {this.BowlsAgainst}+{this.PlusAgainst}, Ends: {this.Ends} Lane: {this.MatchData.Lane + 1})";
        }

        public int Ends { get; set; } = 0;

        public int Bowls => this.MatchScore[this.TeamIndex];

        public int Against => this.MatchScore.Where((item, index) => index != this.TeamIndex).Max();

        public int Rank { get; set; } = -1;

        public int BowlsFor {
            get {
                return Math.Min(this.Limit, this.Bowls);
            }
        }

        /// <summary>
        /// Return the maximum bowls against for all teams in the same match.
        /// </summary>
        public int BowlsAgainst {
            get {
                return this.MatchScore
                           .Where((score, index) => index != this.TeamIndex)
                           .Select(score => Math.Min(this.Limit, score))
                           .Max();
            }
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
            if (cmp != 0) return -cmp; // Higher GameResult wins (Win > Draw > Loss)

            cmp = this.BowlsFor.CompareTo(other.BowlsFor); // Higher is better
            if (cmp != 0) return cmp;

            cmp = this.PlusFor.CompareTo(other.PlusFor); // Higher is better
            if (cmp != 0) return cmp;

            cmp = other.BowlsAgainst.CompareTo(this.BowlsAgainst); // Lower is better
            if (cmp != 0) return -cmp;

            cmp = other.PlusAgainst.CompareTo(this.PlusAgainst); // Lower is better
            if (cmp != 0) return -cmp;

            return 0; // Equal results
        }
    }
}
