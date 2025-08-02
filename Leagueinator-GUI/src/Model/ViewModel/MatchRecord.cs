namespace Leagueinator.GUI.Model {
    public record MatchRecord {
        public MatchFormat MatchFormat { get; }
        public int Ends { get; }
        public int TieBreaker { get; }
        public int Lane { get; }
        public int[] Score { get; }

        public MatchRecord(MatchData matchData) {
            this.MatchFormat = matchData.MatchFormat;
            this.Ends = matchData.Ends;
            this.TieBreaker = matchData.TieBreaker; 
            this.Lane = matchData.Lane;
            this.Score = [.. matchData.Score];
        }

        public override string ToString() {
            return string.Join("|", MatchFormat, Ends, TieBreaker, Lane, string.Join(",", Score));
        }
    }
}
