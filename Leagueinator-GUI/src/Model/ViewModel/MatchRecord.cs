namespace Leagueinator.GUI.Model.ViewModel {
    public record MatchRecord(
        MatchFormat MatchFormat,
        int Ends,
        int TieBreaker,
        int Lane,
        int[] Score
    ) {
        public MatchRecord(MatchData matchData) : this(
            matchData.MatchFormat,
            matchData.Ends,
            matchData.TieBreaker,
            matchData.Lane,
            [.. matchData.Score]
        ) { }

        public override string ToString() {
            return string.Join("|", MatchFormat, Ends, TieBreaker, Lane, string.Join(",", Score));
        }

        public static MatchRecord FromString(string s) {
            var parts = s.Split('|');
            if (parts.Length != 5) throw new FormatException("Invalid MatchRecord string format");

            return new MatchRecord(
                MatchFormat: Enum.Parse<MatchFormat>(parts[0]),
                Ends: int.Parse(parts[1]),
                TieBreaker: int.Parse(parts[2]),
                Lane: int.Parse(parts[3]),
                Score: [.. parts[4].Split(',').Select(int.Parse)]
            );
        }

        public static List<MatchRecord> MatchRecordList(RoundData roundData) {
            List<MatchRecord> matches = [];
            foreach (MatchData match in roundData.Matches) {
                matches.Add(new MatchRecord(match));
            }
            return matches;
        }
    }
}
