using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Leagueinator.GUI.Controllers.Algorithms {
    /// <summary>
    /// Provides logic to assign lanes to matches in a round, ensuring that teams are not repeatedly assigned to the same lanes across rounds.
    /// </summary>
    public class AssignLanes {
        /// <summary>
        /// Gets the event data containing configuration such as lane count and default ends.
        /// </summary>
        private EventData EventData { get; }

        /// <summary>
        /// Gets the collection of all rounds in the event.
        /// </summary>
        private RoundDataCollection Rounds { get; }

        /// <summary>
        /// Gets the round for which lanes are being assigned.
        /// </summary>
        private RoundData TargetRound { get; }

        /// <summary>
        /// Stores the history of lanes used by each team across previous rounds.
        /// </summary>
        private Dictionary<TeamData, HashSet<int>> TeamHistory = [];

        /// <summary>
        /// Stores the history of lanes used by each match (union of teams' lane histories).
        /// </summary>
        private Dictionary<MatchData, HashSet<int>> MatchHistory = [];

        /// <summary>
        /// Gets the final lane assignments for each match after assignment.
        /// </summary>
        public Dictionary<MatchData, int> LaneAssignments { get; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignLanes"/> class.
        /// </summary>
        /// <param name="eventData">The event configuration data.</param>
        /// <param name="rounds">The collection of all rounds in the event.</param>
        /// <param name="target">The round to assign lanes for.</param>
        public AssignLanes(EventData eventData, RoundDataCollection rounds, RoundData target) {
            this.EventData = eventData;
            this.Rounds = rounds;
            this.TargetRound = target;
        }

        /// <summary>
        /// Performs the lane assignment for the target round, ensuring teams are not assigned to the same lanes 
        /// as in previous rounds.
        /// Creates a new <see cref="RoundData"/> instance with the assigned lanes using the team assignements
        /// from the target round.
        /// </summary>
        /// <returns>A new <see cref="RoundData"/> instance with updated lane assignments.</returns>
        public RoundData DoAssignment() {
            this.BuildTeamHistory();
            this.BuildMatchHistory();
            this.InvertMatchHistory();
            return this.DoAssignLanes();
        }

        /// <summary>
        /// Assigns lanes to matches in the target round, avoiding previously used lanes for each match.
        /// </summary>
        /// <returns>A new <see cref="RoundData"/> with assigned lanes.</returns>
        private RoundData DoAssignLanes() {
            RoundData newRound = [];
            HashSet<int> usedLanes = [];

            var sorted = MatchHistory
                .OrderBy(kvp => kvp.Value.Count)
                .ToList();

            foreach (KeyValuePair<MatchData, HashSet<int>> kvp in sorted) {
                MatchData match = kvp.Key;
                HashSet<int> lanes = kvp.Value;
                Debug.WriteLine($"{string.Join(", ", match.Players)}, {string.Join(", ", lanes)}");
            }

            while (sorted.Count > 0) {
                var kvp = sorted[0];
                sorted.RemoveAt(0);

                MatchData match = kvp.Key;
                HashSet<int> lanes = kvp.Value;

                if (lanes.Count == 0) {
                    throw new AlgoLogicException($"No available lanes for players: {string.Join(", ", match.Players)}");
                }

                int lane = lanes.First();
                usedLanes.Add(lane);

                MatchData newMatch = new MatchData(match.MatchFormat) {
                    Teams = [.. match.Teams.Select(t => t.Copy())],
                    Lane = lane,
                    Ends = match.Ends,
                    Score = [.. match.Score],
                };

                newRound.Add(newMatch);
                sorted = this.RemoveFromKVP(sorted, lane);

            }
            newRound.Sort((a, b) => a.Lane.CompareTo(b.Lane));
            newRound.Fill(this.EventData);
            return newRound;
        }


        private List<KeyValuePair<MatchData, HashSet<int>>> RemoveFromKVP(
            List<KeyValuePair<MatchData, HashSet<int>>> list, int value) {

            foreach (var kvp in list) {
                kvp.Value.Remove(value);
            }

            return list.OrderBy(kvp => kvp.Value.Count).ToList();
        }

        /// <summary>
        /// Inverts the match history so that each match's set contains lanes that have not yet been used by its teams.
        /// </summary>
        private void InvertMatchHistory() {
            foreach (MatchData match in this.MatchHistory.Keys) {
                HashSet<int> inverse = Enumerable.Range(0, this.EventData.LaneCount).ToHashSet();
                inverse.ExceptWith(this.MatchHistory[match]);
                this.MatchHistory[match] = inverse;
            }
        }

        /// <summary>
        /// Builds the match history by aggregating the lane usage history of all teams in each match.
        /// </summary>
        private void BuildMatchHistory() {
            foreach (MatchData match in this.TargetRound) {
                if (match.CountPlayers() == 0) continue;
                this.MatchHistory[match] = [];

                foreach (TeamData team in match.Teams) {
                    this.MatchHistory[match].UnionWith(this.TeamHistory[team]);
                }
            }

            if (this.MatchHistory.Count == 0) {
                throw new InvalidOperationException("No matches found in the target round.");
            }
            else {
                Debug.WriteLine($"Match history built with {this.MatchHistory.Count} matches.");
            }
        }

        /// <summary>
        /// Builds the lane usage history for each team, excluding the target round.
        /// </summary>
        public Dictionary<TeamData, HashSet<int>> BuildTeamHistory() {
            foreach (TeamData team in this.Rounds.Teams) {
                this.TeamHistory[team] = [];
            }

            foreach (RoundData round in this.Rounds) {
                if (round == this.TargetRound) continue; // Skip the current round

                foreach (MatchData match in round) {
                    foreach (TeamData team in match.Teams) {
                        this.TeamHistory[team].Add(match.Lane);
                    }
                }
            }

            return this.TeamHistory;
        }
    }
}
