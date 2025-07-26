namespace Leagueinator.GUI.Model {
    public record Record {
        public readonly EventData? EventData;
        public readonly RoundData RoundData;
        public readonly MatchData MatchData;
        public readonly TeamData TeamData;
        public readonly string Name;

        public Record(EventData eventData, RoundData roundData, MatchData matchData, TeamData teamData, string name) {
            this.EventData = eventData;
            this.RoundData = roundData;
            this.MatchData = matchData;
            this.TeamData = teamData;
            this.Name = name;
        }

        public Record(RoundData roundData, MatchData matchData, TeamData teamData, string name) {
            this.RoundData = roundData;
            this.MatchData = matchData;
            this.TeamData = teamData;
            this.Name = name;
        }
    }
}
