namespace Leagueinator.GUI.Model {
    public record RoundRecord{
        public required int EventUID;
        public required int Round;
        public required int Lane;
        public required int Team;
        public required int Pos;
        public required string Name;

        public override string ToString() {
            return string.Join("|", EventUID, Round, Lane, Team, Pos, Name);
        }

        public static RoundRecord FromString(string s) {
            var parts = s.Split('|');
            if (parts.Length != 6) throw new FormatException("Invalid RoundRecord string format");

            return new RoundRecord {
                EventUID = int.Parse(parts[0]),
                Round = int.Parse(parts[1]),
                Lane = int.Parse(parts[2]),
                Team = int.Parse(parts[3]),
                Pos = int.Parse(parts[4]),
                Name = parts[5]
            };
        }
    }
}
