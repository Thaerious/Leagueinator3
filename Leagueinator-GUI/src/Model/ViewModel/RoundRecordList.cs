namespace Leagueinator.GUI.Model {
    public class RoundRecordList {
        public List<RoundRecord> Players { get; } = [];
        public Dictionary<int, MatchRecord> Matches { get; } = [];

        public RoundRecordList(EventData @event, RoundData round) {
            foreach (MatchData match in round) {
                this.Matches[round.IndexOf(match)] = new() {
                    MatchFormat = match.MatchFormat,
                    Ends = match.Ends,
                    TieBreaker = match.TieBreaker,
                    Lane = round.IndexOf(match),
                    Score = match.Score,
                };

                foreach (TeamData team in match.Teams) {
                    foreach (string player in team) {
                        if (player == string.Empty) continue;   
                        RoundRecord record = new() {
                            EventUID = @event.UID,
                            Round = @event.IndexOf(round),
                            Lane = round.IndexOf(match),
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
