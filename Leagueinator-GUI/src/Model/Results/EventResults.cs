using Leagueinator.GUI.Model;
using System.Diagnostics;

namespace Leagueinator.GUI.Model.Results {

    /// <summary>
    /// Compares two string arrays for equality by checking element-wise equivalence.
    /// Used to compare player team compositions in dictionaries.
    /// </summary>
    class StringArrayComparer : IEqualityComparer<string[]> {
        /// <summary>
        /// Determines whether the specified arrays are equal.
        /// </summary>
        /// <param name="x">The first array.</param>
        /// <param name="y">The second array.</param>
        /// <returns><c>true</c> if the arrays are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(string[]? x, string[]? y) =>
            x != null && y != null && x.SequenceEqual(y);

        /// <summary>
        /// Returns a hash code for the specified array.
        /// </summary>
        /// <param name="obj">The array for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified array.</returns>
        public int GetHashCode(string[] obj) {
            int hash = 17;
            foreach (var s in obj)
                hash = hash * 31 + (s?.GetHashCode() ?? 0);
            return hash;
        }
    }

    /// <summary>
    /// Aggregates results across all rounds of an event.
    /// </summary>
    internal class EventResults {

        /// <summary>
        /// Results grouped by each round in the event.
        /// </summary>
        public List<RoundResults> ResultsByRound { get; } = [];

        /// <summary>
        /// Results aggregated by each unique team across all rounds.
        /// The results are sorted in descending order of performance.
        /// </summary>
        public List<TeamResult> ResultsByTeam { get; } = [];

        /// <summary>
        /// Constructs event-level results from a collection of round data.
        /// </summary>
        /// <param name="roundDataCollection">The collection of rounds to analyze.</param>
        public EventResults(RoundDataCollection roundDataCollection) {
            // Compute round-level results
            foreach (var round in roundDataCollection) {
                ResultsByRound.Add(new RoundResults(round));
            }

            Dictionary<TeamData, TeamResult> teamResultsMap = [];

            // Aggregate results by team
            foreach (RoundResults roundResults in this.ResultsByRound) {
                foreach (SingleResult singleResult in roundResults.Results) {
                    if (teamResultsMap.ContainsKey(singleResult.TeamData) == false) {
                        teamResultsMap.Add(singleResult.TeamData, new TeamResult(singleResult.TeamData));
                    }

                    TeamResult teamResults = teamResultsMap[singleResult.TeamData];
                    teamResults.MatchResults.Add(singleResult);
                }
            }

            // Sort teams by performance and assign ranks
            var results = teamResultsMap.Values.ToList();
            results.Sort();
            results.Reverse();

            for (int i = 0; i < results.Count; i++) {
                TeamResult teamResult = results[i];
                teamResult.Rank = i + 1; // Rank starts at 1
            }

            this.ResultsByTeam = results;
        }
    }
}
