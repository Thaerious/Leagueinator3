using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Text;

namespace Leagueinator.GUI.Model {

    public record RoundRecord{
        public required int Round;
        public required int Lane;
        public required int Team;
        public required int Pos;
        public required string Name;
    }

    public record MatchRecord {
        public required MatchFormat MatchFormat;
        public required int Ends;
        public required int TieBreaker;
    }

    public class RoundRecordList {
        public List<RoundRecord> Players { get; } = [];
        public Dictionary<int, MatchRecord> Matches { get; } = [];

        public RoundRecordList(EventData @event, RoundData round) {
            foreach (MatchData match in round) {
                this.Matches[round.IndexOf(match)] = new() {
                    MatchFormat = match.MatchFormat,
                    Ends = match.Ends,
                    TieBreaker = match.TieBreaker,
                };

                foreach (TeamData team in match.Teams) {
                    foreach (string player in team) {
                        if (player == string.Empty) continue;   
                        RoundRecord record = new() {
                            Round = @event.Rounds.IndexOf(round),
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

    public class RoundData : List<MatchData> {
        
        public RoundData() : base() {}

        public RoundData(EventData eventData){
            this.Fill(eventData);
        }

        public void Fill(EventData eventData) {
            for (int i = 0; i < eventData.LaneCount; i++) {
                if (!this.Any(m => m.Lane == i)) {
                    this.Add(new MatchData(eventData.MatchFormat) {
                        Lane = i,
                        Ends = eventData.DefaultEnds
                    });
                }
            }   

            this.Sort((a, b) => a.Lane.CompareTo(b.Lane));
        }

        public RoundData Copy() { 
            RoundData roundCopy = [];

            foreach (MatchData match in this) {
                roundCopy.Add(match.Copy());
            }

            return roundCopy;
        }

        public override string ToString() {
            StringBuilder sb = new();
            foreach (MatchData match in this) {
                sb.Append(match.ToString());
                sb.Append('\n');
            }
            return sb.ToString();
        }

        public void RemovePlayer(string name) {
            if (string.IsNullOrEmpty(name)) {
                return;
            }

            foreach (MatchData match in this) {
                match.RemoveName(name);
            }
        }

        public void SetPlayer(string name, int lane, int teamIndex, int position) {
            Debug.WriteLine($"SetPlayer: name={name}, lane={lane}, team={teamIndex}, pos={position}");
            if (this.HasPlayer(name)) {
                this.RemovePlayer(name);
            }

            this[lane].Teams[teamIndex][position] = name;
        }

        public bool HasPlayer(string name) {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            foreach (MatchData match in this) {
                foreach (TeamData team in match.Teams) {
                    if (team.Names.Contains(name)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public (int, int, int) PollPlayer(string name) {
            foreach (MatchData match in this) {
                foreach (TeamData team in match.Teams) {
                    if (team.Names.Contains(name)) {
                        return (match.Lane, Array.IndexOf(match.Teams, team), team.IndexOf(name));
                    }
                }
            }
            return (-1, -1, -1);
        }

        internal void AssignPlayersRandomly() {
            var dict = new Dictionary<(int, int, int), string>();

            // Populate the dictionary with player Names and their positions
            foreach (MatchData match in this) {
                for (int team = 0; team < match.Teams.Length; team++) {
                    for (int position = 0; position < match.Teams[team].Length; position++) {
                        if (!string.IsNullOrEmpty(match.Teams[team][position])) {
                            dict[(match.Lane, team, position)] = match.Teams[team][position];
                        }
                    }
                }
            }

            // Extract keys and values
            var keys = dict.Keys.ToList();
            var values = dict.Values.ToList();

            // Shuffle values
            var rng = new Random();
            values = values.OrderBy(_ => rng.Next()).ToList();

            // Reassign shuffled values back to their original positions
            foreach (var key in keys) {
                var lane = key.Item1;
                var team = key.Item2;
                var position = key.Item3;   

                this[lane].Teams[team][position] = values.Pop();
            }
        }
    }
}
