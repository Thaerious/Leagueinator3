namespace Leagueinator.GUI.Model {
    public record MatchRecord {
        public required MatchFormat MatchFormat;
        public required int Ends;
        public required int TieBreaker;
        public required int Lane;
        public required int[] Score;

        public override string ToString() {
            return string.Join("|", MatchFormat, Ends, TieBreaker, Lane, string.Join(",", Score));
        }

        public static MatchRecord FromString(string s) {
            var parts = s.Split('|');
            if (parts.Length != 5) throw new FormatException("Invalid MatchRecord string format");

            return new MatchRecord {
                MatchFormat = Enum.Parse<MatchFormat>(parts[0]),
                Ends = int.Parse(parts[1]),
                TieBreaker = int.Parse(parts[2]),
                Lane = int.Parse(parts[3]),
                Score = [.. parts[4].Split(',').Select(int.Parse)]
            };
        }
    }
}
