using System.Diagnostics;
using System.Xml.Linq;

namespace Leagueinator.GUI.Model.ViewModel {
    /// <summary>
    /// A record representing a round in an event.
    /// Contains non-reflective data for a single player in a single match.
    /// </summary>
    public record PlayerRecord{
        public string EventName { get; }
        public int Round { get; }
        public MatchFormat MatchFormat { get; }
        public int Ends { get; }
        public int TieBreaker { get; }
        public int[] Score { get; }
        public int Lane { get; }
        public int Team { get; }
        public int PlayerPos { get; }
        public string Name { get; }

        public PlayerRecord(TeamData teamData, int playerPos) {
            this.EventName = teamData.Ancestor<EventData>()?.EventName ?? throw new NullReferenceException();
            this.Round = teamData.Ancestor<RoundData>()?.Index ?? throw new NullReferenceException();
            this.MatchFormat = teamData.Parent.MatchFormat;
            this.Ends = teamData.Parent.Ends;
            this.TieBreaker = teamData.Parent.TieBreaker;
            this.Score = [..teamData.Parent.Score];
            this.Lane = teamData.Parent.Lane;
            this.Team = teamData.Index;
            this.PlayerPos = playerPos;
            this.Name = teamData.Names[playerPos];
        }

        // Alternate constructor for deserialization
        public PlayerRecord(string eventName, int round, MatchFormat matchFormat, int ends, int tieBreaker,
                            int[] score, int lane, int team, int playerPos, string name) {
            
            this.EventName = eventName;
            this.Round = round;
            this.MatchFormat = matchFormat;
            this.Ends = ends;
            this.TieBreaker = tieBreaker;
            this.Score = score;
            this.Lane = lane;
            this.Team = team;
            this.PlayerPos = playerPos;
            this.Name = name;
        }

        public override string ToString() {
            return string.Join("|",
                EventName,
                Round,
                MatchFormat,
                Ends,
                TieBreaker,
                string.Join(",", Score),
                Lane,
                Team,
                PlayerPos,
                Name.Replace("|", "").Replace(",", "") // sanitize to avoid breaking the format
            );
        }

        public static PlayerRecord FromString(string s) {
            var parts = s.Split('|');
            if (parts.Length != 11) throw new FormatException("Invalid PlayerRecord string format");

            return new PlayerRecord(
                eventName: parts[0],
                round: int.Parse(parts[1]),
                matchFormat: Enum.Parse<MatchFormat>(parts[2]),
                ends: int.Parse(parts[3]),
                tieBreaker: int.Parse(parts[4]),
                score: parts[5].Split(',').Select(int.Parse).ToArray(),
                lane: int.Parse(parts[6]),
                team: int.Parse(parts[7]),
                playerPos: int.Parse(parts[8]),
                name: parts[9]
            );
        }

        public string ToCSV() {
            return string.Join(",",
                EventName,
                Round,
                MatchFormat,
                Ends,
                TieBreaker,
                string.Join(";", Score), // Avoid CSV conflicts with `,`
                Lane,
                Team,
                PlayerPos,
                $"\"{Name}\"" // quote in case name has spaces
            );
        }
    }
}
