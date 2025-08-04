namespace Leagueinator.GUI.Model.ViewModel {
    public record EventRecord(
        string Name,
        DateTime Created,
        MatchFormat MatchFormat,
        int LaneCount,
        int DefaultEnds,
        EventType EventType,
        int RoundCount
    ) {

        public override string ToString() {
            return string.Join("|", new string[] {
                Name,
                Created.ToString("o"), // ISO 8601 for safe DateTime format
                MatchFormat.ToString(),
                LaneCount.ToString(),
                DefaultEnds.ToString(),
                EventType.ToString(),
                RoundCount.ToString(),
            });
        }

        public static EventRecord FromString(string s) {
            var parts = s.Split('|');
            if (parts.Length != 7) throw new FormatException("Invalid EventRecord string format");

            return new EventRecord(
                Name: parts[0],
                Created: DateTime.Parse(parts[1], null, System.Globalization.DateTimeStyles.RoundtripKind),
                MatchFormat: Enum.Parse<MatchFormat>(parts[2]),
                LaneCount: int.Parse(parts[3]),
                DefaultEnds: int.Parse(parts[4]),
                EventType: Enum.Parse<EventType>(parts[5]),
                RoundCount: int.Parse(parts[6])
            );
        }
    }
}
