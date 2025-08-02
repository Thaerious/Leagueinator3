namespace Leagueinator.GUI.Model {


    /// <summary>
    /// A record representing a round in an event.
    /// Contains non-reflective for a single player in a single match.
    /// </summary>
    public record RoundRecord{
        public required string EventName;
        public required int Round;
        public required int Lane;
        public required int Team;
        public required int Pos;
        public required string Name;

        public override string ToString() {
            return string.Join("|", EventName, Round, Lane, Team, Pos, Name);
        }

        public static RoundRecord FromString(string s) {
            var parts = s.Split('|');
            if (parts.Length != 6) throw new FormatException("Invalid RoundRecord string format");

            return new RoundRecord {
                EventName = parts[0],
                Round = int.Parse(parts[1]),
                Lane = int.Parse(parts[2]),
                Team = int.Parse(parts[3]),
                Pos = int.Parse(parts[4]),
                Name = parts[5]
            };
        }
    }
}
