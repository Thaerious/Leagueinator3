using Leagueinator.GUI.Model.Enums;
using Leagueinator.GUI.Model.ViewModel;
using System.Collections;
using System.IO;
using Utility.Collections;

namespace Leagueinator.GUI.Model {

    /// <summary>
    /// Represents a single event (e.g. tournament night, league day) within a <see cref="LeagueData"/>.
    /// Stores metadata (name, date, format, lanes, ends, type) and a collection of <see cref="RoundData"/>.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IEnumerable{RoundData}"/> for iteration over rounds.
    /// Supports serialization to and from text streams for persistence.
    /// </remarks>
    public class EventData(LeagueData LeagueData) : IHasTeams, IEnumerable<RoundData>, IHasParent<LeagueData> {
        /// <summary>
        /// Parent league this event belongs to.
        /// </summary>
        public LeagueData Parent { get; } = LeagueData;

        /// <summary>
        /// Display name of the event. Defaults to current date in "MMMM d, yyyy" format.
        /// </summary>
        public string EventName { get; set; } = DateTime.Now.ToString("MMMM d, yyyy");

        /// <summary>
        /// Date the event took place or is scheduled.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Default match format (e.g. 2v2, 3v3).
        /// </summary>
        public MatchFormat DefaultMatchFormat { get; set; } = MatchFormat.VS2;

        /// <summary>
        /// Number of lanes available for this event.
        /// </summary>
        public int LaneCount { get; set; } = 8;

        /// <summary>
        /// Default number of ends per match.
        /// </summary>
        public int DefaultEnds { get; set; } = 10;

        /// <summary>
        /// Type of event (e.g. Swiss, Jitney, etc.).
        /// </summary>
        public EventType EventType { get; set; } = EventType.Swiss;

        /// <summary>
        /// Type of scoring, Bowls or Plus.
        /// Defaults to Plus.
        /// </summary>
        public MatchScoring MatchScoring { get; set; } = MatchScoring.Plus;

        /// <summary>
        /// Whether the first tiebreaker is head to head.
        /// Defaults to false.
        /// </summary>
        public bool HeadToHeadScoring { get; set; } = false;

        /// <summary>
        /// Backing list for rounds in this event.
        /// </summary>
        private readonly List<RoundData> _rounds = [];

        /// <summary>
        /// Read-only access to rounds.
        /// </summary>
        public IReadOnlyList<RoundData> Rounds => _rounds;

        /// <summary>
        /// Creates and adds a new round to the event.
        /// </summary>
        /// <param name="fill">Whether to auto-populate the round with default matches.</param>
        /// <returns>The newly added round.</returns>
        public RoundData AddRound(bool fill = true) {
            RoundData newRound = new(this);
            if (fill) newRound.Fill();
            this._rounds.Add(newRound);
            return newRound;
        }

        /// <summary>
        /// Adds an existing round to this event, validating parent relationship.
        /// </summary>
        /// <exception cref="InvalidParentException">Thrown if round belongs to a different event.</exception>
        public RoundData AddRound(RoundData roundData) {
            if (roundData.Parent != this) throw new InvalidParentException();
            this._rounds.Add(roundData);
            return roundData;
        }

        /// <summary>
        /// Returns the index of a given round.
        /// </summary>
        public int IndexOf(RoundData roundData) => this._rounds.IndexOf(roundData);

        /// <summary>
        /// Returns all non-empty teams that participated in this event.
        /// This returns an object for each team in each match.
        /// </summary>
        public IEnumerable<TeamData> AllTeams() {
            return this.Rounds.SelectMany(r => r.AllTeams());
        }

        /// <summary>
        /// Returns a list of all matches that a team (specified by players) has played in.
        /// </summary>
        /// <param name="players">Names of the players forming the team.</param>
        public List<MatchData> GetMatchesForTeam(IEnumerable<string> players) {
            List<MatchData> matches = [.. this.Rounds.SelectMany(r => r.Matches)];

            foreach (RoundData round in this) {
                foreach (MatchData match in round.Matches) {
                    foreach (TeamData consideringTeam in match.Teams) {
                        if (consideringTeam.Equals(players)) {
                            matches.Add(match);
                        }
                    }
                }
            }
            return matches;
        }

        /// <summary>
        /// Enumerator for rounds in this event.
        /// </summary>
        public IEnumerator<RoundData> GetEnumerator() => this._rounds.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Removes the round at the specified index.
        /// </summary>
        internal void RemoveRound(int index) => this._rounds.RemoveAt(index);

        /// <summary>
        /// Determines whether <paramref name="team"/> has played against <paramref name="opponent"/>.
        /// True if any player overlaps with a previously faced opponent.
        /// </summary>
        public bool HasPlayed(Players team, Players opponent) {
            foreach (TeamData prevOpponent in this.PreviousOpponents(team)) {
                if (opponent.Intersect(prevOpponent.Names).Any()) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns player records aggregated across all rounds.
        /// </summary>
        public IEnumerable<PlayerRecord> Records() {
            return this.SelectMany(roundData => roundData.Records());
        }
               

        /// <summary>
        /// Returns all previous opponents for the specified players.
        /// </summary>
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
        /// Returns all previous opponents for the specified players.
        /// </summary>
        public MultiMap<Players, Players> PreviousOpponents() {
            MultiMap<Players, Players> previousOpponents = [];

            foreach (TeamData teamData in this.AllTeams()) {
                if (teamData.IsEmpty()) continue;
                foreach (TeamData opponent in teamData.GetOpposition()) {
                    previousOpponents[teamData.ToPlayers()].Add(opponent.ToPlayers());
                }
            }

            return previousOpponents;
        }

        public MultiMap<Players, int> PreviousLanes(RoundData? except = null) {
            MultiMap<Players, int> previousLanes = [];

            foreach (TeamData team in this.AllTeams()) {
                if (except != null && except == team.Parent.Parent) continue;
                previousLanes.Add(team.ToPlayers(), team.Parent.Lane);
            }

            return previousLanes;
        }

        /// <summary>
        /// Replaces an existing round with a new one at the same index.
        /// </summary>
        internal void ReplaceRound(RoundData roundData, RoundData newRound) {
            int index = this._rounds.IndexOf(roundData);
            this._rounds[index] = newRound;
        }
    }
}
