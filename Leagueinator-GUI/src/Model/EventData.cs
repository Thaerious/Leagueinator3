using Leagueinator.GUI.Model.ViewModel;
using System.Collections;

namespace Leagueinator.GUI.Model {

    public class EventData(LeagueData LeagueData) : IEnumerable<RoundData>, IHasParent<LeagueData> {
        public LeagueData Parent { get; } = LeagueData;
        public string EventName { get; set; } = DateTime.Now.ToString("MMMM d, yyyy");
        public DateTime Date { get; set; } = DateTime.Now;
        public MatchFormat DefaultMatchFormat { get; set; } = MatchFormat.VS2;
        public int DefaultLaneCount { get; set; } = 8;
        public int DefaultEnds { get; set; } = 10;
        public EventType EventType { get; set; } = EventType.RankedLadder;

        private readonly List<RoundData> _rounds = [];
        public IReadOnlyList<RoundData> Rounds => _rounds;

        public static EventRecord ToRecord(EventData data) {
            return new EventRecord(
                data.EventName,
                data.Date,
                data.DefaultMatchFormat,
                data.DefaultLaneCount,
                data.DefaultEnds,
                data.EventType,
                data.Rounds.Count
            );
        }

        public void ReplaceRound(int index, RoundData roundData) {
            if (roundData.Parent != this) throw new InvalidParentException();
            this._rounds[index] = roundData;
        }

        public RoundData AddRound(bool fill = true) {
            RoundData newRound = new(this);
            if (fill) newRound.Fill();
            this._rounds.Add(newRound);
            return newRound;
        }

        public RoundData AddRound(RoundData roundData) {
            if (roundData.Parent != this) throw new InvalidParentException();
            this._rounds.Add(roundData);
            return roundData;
        }

        public int IndexOf(RoundData roundData) => this._rounds.IndexOf(roundData);

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
        /// Returns a list of all matches that a players has played in.
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public List<MatchData> GetMatchesForTeam(IEnumerable<string> players) {
            List<MatchData> matches = [];

            foreach (RoundData round in this) {
                foreach (MatchData match in round) {
                    foreach (TeamData consideringTeam in match.Teams) {
                        if (consideringTeam.Equals(players)) {
                            matches.Add(match);
                        }
                    }
                }
            }
            return matches;
        }

        public IEnumerator<RoundData> GetEnumerator() {
            return this._rounds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        internal void RemoveRound(int index) => this._rounds.RemoveAt(index);

        /// <summary>
        /// Returns a list of all previous previous opponents for the Target players.
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public List<TeamData> PreviousOpponents(IEnumerable<string> players) {
            List<TeamData> opponents = [];

            foreach (MatchData match in this.GetMatchesForTeam(players)) {
                foreach (TeamData opposingTeam in match.Teams) {
                    if (opposingTeam.Equals(players)) continue; // Skip our own players
                    opponents.Add(opposingTeam);
                }
            }

            return opponents;
        }

        /// <summary>
        /// True if any player from players has played against any player from pollOpponent.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public bool HasPlayed(IEnumerable<string> team, IEnumerable<string> opponent) {
            foreach (TeamData prevOpponent in this.PreviousOpponents(team)) {
                if (opponent.Intersect(prevOpponent.Names).Any()) {
                    return true;
                }
            }
            return false;
        }

        public static EventData FromRecord(LeagueData leagueData, EventRecord record) {
            return new(leagueData) {
                EventName = record.Name,
                Date = record.Created,
                DefaultMatchFormat = record.MatchFormat,
                DefaultEnds = record.DefaultEnds,
                DefaultLaneCount = record.LaneCount,
                EventType = record.EventType,
            };
        }

        public IEnumerable<PlayerRecord> Records() {
            return this.SelectMany(roundData => roundData.Records());
        }
    }
}
