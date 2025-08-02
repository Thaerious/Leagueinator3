namespace Leagueinator.GUI.Model {
    public class RoundRecordList {
        public List<RoundRecord> Players { get; } = [];
        public List<MatchRecord> Matches { get; } = [];

        public RoundRecordList(EventData @event, RoundData round) {
            foreach (MatchData match in round) {
                this.Matches.Add(new MatchRecord(match));

                foreach (TeamData team in match.Teams) {
                    foreach (string player in team) {
                        if (player == string.Empty) continue;   
                        RoundRecord record = new() {
                            EventName = @event.EventName,
                            Round = @event.IndexOf(round),
                            Lane = match.Lane,
                            Team = Array.IndexOf(match.Teams, team),
                            Pos = Array.IndexOf(team.Names, player),
                            Name = player
                        };
                        Players.Add(record);
                    }
                }
            }
        }
    }
}
