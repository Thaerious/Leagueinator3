namespace Leagueinator.GUI.Model.ViewModel {
    /// <summary>
    /// A record representing a round in an event.
    /// Contains non-reflective data for a single player in a single match.
    /// </summary>
    public record PlayerRecord(
        string EventName,
        int Round,
        int Lane,
        int Team,
        int Pos,
        string Name
    ) {
        public override string ToString() {
            return string.Join("|", EventName, Round, Lane, Team, Pos, Name);
        }

        public string ToCSV() {
            return string.Join(",", EventName, Round, Lane, Team, Pos, Name);
        }

        public static PlayerRecord FromString(string s) {
            var parts = s.Split('|');
            if (parts.Length != 6) throw new FormatException("Invalid Record string format");

            return new PlayerRecord(
                EventName: parts[0],
                Round: int.Parse(parts[1]),
                Lane: int.Parse(parts[2]),
                Team: int.Parse(parts[3]),
                Pos: int.Parse(parts[4]),
                Name: parts[5]
            );
        }
    }
}
