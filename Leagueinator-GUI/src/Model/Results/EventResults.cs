
namespace Leagueinator.GUI.Model.Results {
    /// <summary>
    /// Aggregates results across all rounds of an event.
    /// </summary>
    public class EventResults {
        /// <summary>
        /// Constructs event-level results from a collection of round data.
        /// </summary>
        /// <param name="roundDataCollection">The collection of rounds to analyze.</param>
        public EventResults(IEnumerable<RoundData> rounds) {
            // Compute round-level results
            foreach (var round in rounds) {
                ResultsByRound.Add(new RoundResults(round));                
            }

            var teamResultsMap = this.AggregateResultsByTeam(rounds);

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

        /// <summary>
        /// Create a map from tem data to their aggregated team results.
        /// The aggregated results include all matches played by the team across all rounds.
        /// </summary>
        /// <param name="roundDataCollection"></param>
        /// <returns></returns>
        private Dictionary<TeamData, TeamResult> AggregateResultsByTeam(IEnumerable<RoundData> rounds) {
            Dictionary<TeamData, TeamResult> teamResultsMap = [];

            foreach (RoundResults roundResults in this.ResultsByRound) {
                foreach (SingleResult singleResult in roundResults.Results) {
                    if (teamResultsMap.ContainsKey(singleResult.TeamData) == false) {
                        teamResultsMap.Add(singleResult.TeamData, new TeamResult(singleResult.TeamData));
                    }

                    TeamResult teamResults = teamResultsMap[singleResult.TeamData];
                    teamResults.MatchResults.Add(singleResult);
                }
            }

            return teamResultsMap;
        }

        /// <summary>
        /// Results grouped by each round in the event.
        /// </summary>
        public List<RoundResults> ResultsByRound { get; } = [];

        /// <summary>
        /// Results aggregated by each unique team across all rounds.
        /// The results are sorted in descending order of performance.
        /// </summary>
        public List<TeamResult> ResultsByTeam { get; } = [];
    }
}
