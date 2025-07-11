using Leagueinator.GUI.Controllers.Algorithms;
using System.Collections;

namespace Leagueinator.GUI.Model {

    public class EventData : IEnumerable<RoundData> {
        public string EventName { get; set; } = "Name Not Set";
        public DateTime Date { get; set; } = DateTime.Now;
        public MatchFormat MatchFormat { get; set; } = MatchFormat.VS2;
        public int LaneCount { get; set; } = 8;
        public int DefaultEnds { get; set; } = 10;
        public EventType EventType { get; set; } = EventType.RankedLadder;
        private List<RoundData> Rounds { get; set; } = [];

        required public int UID { get; set; }

        public RoundData GetRound(int index) => this.Rounds[index];

        public int CountRounds() => this.Rounds.Count;

        public static EventRecord ToRecord(EventData data) {
            return new EventRecord(
                data.EventName,
                data.Date,
                data.MatchFormat,
                data.LaneCount,
                data.DefaultEnds,
                data.EventType,
                data.Rounds.Count,
                data.UID
            );
        }

        public EventData Copy() {
            var copy = new EventData {
                EventName = this.EventName,
                Date = this.Date,
                MatchFormat = this.MatchFormat,
                LaneCount = this.LaneCount,
                DefaultEnds = this.DefaultEnds,
                EventType = this.EventType,
                UID = this.UID
            };

            foreach (var round in this.Rounds) {
                copy.Rounds.Add(round.Copy()); // assumes Rounds.Copy() already returns a deep copy
            }

            return copy;
        }

        public void SetRound(int index, RoundData roundData) {
            this.Rounds[index] = roundData;
        }

        public RoundData AddRound(bool fill = true) {
            RoundData newRound = new();
            if (fill) newRound.Fill(this);
            this.Rounds.Add(newRound);
            return newRound;
        }

        public RoundData AddRound(RoundData newRound) {
            this.Rounds.Add(newRound);
            return newRound;
        }

        public int IndexOf(RoundData roundData) => this.Rounds.IndexOf(roundData);

        public HashSet<TeamData> Teams {
            get {
                HashSet<TeamData> teams = [];

                foreach (RoundData round in this) {
                    foreach (MatchData match in round) {
                        foreach (TeamData team in match.Teams) {
                            teams.Add(team);
                        }
                    }
                }

                return teams;
            }
        }

        /// <summary>
        /// Returns a list of all matches that a team has played in.
        /// </summary>
        /// <param name="targetTeam"></param>
        /// <returns></returns>
        public List<MatchData> GetMatchesForTeam(TeamData targetTeam) {
            List<MatchData> matches = [];

            foreach (RoundData round in this) {
                foreach (MatchData match in round) {
                    foreach (TeamData consideringTeam in match.Teams) {
                        if (consideringTeam.Equals(targetTeam)) {
                            matches.Add(match);
                        }
                    }
                }
            }
            return matches;
        }

        public RoundData GenerateRound() {

            switch (this.EventType) {
                case EventType.RankedLadder:
                    var newRound = new RankedLadder(this).GenerateRound();
                    this.Rounds.Add(newRound);
                    return newRound;
                //case EventType.RoundRobin:
                //    break;
                //case EventType.Motley:
                //    break;
                default:
                    throw new NotSupportedException($"Match format '{this.MatchFormat}' is not supported.");
            }
        }

        public IEnumerator<RoundData> GetEnumerator() {
            return this.Rounds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        internal void RemoveRound(int index) => this.Rounds.RemoveAt(index);

        /// <summary>
        /// Returns a list of all previous previous opponents for the Target team.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public List<TeamData> PreviousOpponents(TeamData team) {
            List<TeamData> opponents = [];

            foreach (MatchData match in this.GetMatchesForTeam(team)) {
                foreach (TeamData opposingTeam in match.Teams) {
                    if (opposingTeam.Equals(team)) continue; // Skip our own targetTeam
                    opponents.Add(opposingTeam);
                }
            }

            return opponents;
        }

        /// <summary>
        /// True if any player from targetTeam has played against any player from pollOpponent.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public bool HasPlayed(TeamData team, TeamData pollOpponent) {
            foreach (TeamData prevOpponent in this.PreviousOpponents(team)) {
                if (pollOpponent.Names.Intersect(prevOpponent.Names).Any()) {
                    return true;
                }
            }
            return false;
        }

        public static EventData FromRecord(EventRecord record) {
            return new() {
                UID = record.UID,
                EventName = record.Name,
                Date = record.Created,
                MatchFormat = record.MatchFormat,
                DefaultEnds = record.DefaultEnds,
                LaneCount = record.LaneCount,
                EventType = record.EventType,
            };
        }
    }
}
