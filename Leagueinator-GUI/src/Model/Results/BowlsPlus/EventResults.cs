
using Utility;
using Utility.Collections;

namespace Leagueinator.GUI.Model.Results.BowlsPlus {
    /// <summary>
    /// Aggregates results across all _rounds of an event.
    /// </summary>
    public class EventResults {

        /// <summary>
        /// AllResults grouped by each round in the event.
        /// </summary>
        public List<RoundResults> ByRound { get; } = [];

        /// <summary>
        /// AllResults aggregated by each unique team across all _rounds.
        /// The results are sorted in descending order of performance.
        /// </summary>
        public DefaultDictionary<Players, TeamResult> ByTeam { get; } = new((players)=>new(players));

        /// <summary>
        /// Constructs event-level results from a collection of round data.
        /// </summary>
        /// <param name="roundDataCollection">The collection of _rounds to analyze.</param>
        public EventResults(IEnumerable<RoundData> rounds) {
            // Compute round-level results
            foreach (var round in rounds) {
                RoundResults roundResults = new(round);
                ByRound.Add(roundResults);

                foreach (SingleResult result in roundResults.AllResults) {
                    TeamResult teamResult = ByTeam[result.Players];
                    teamResult.Add(result);
                }
            }

            // Sort teams by performance and assign ranks
            List<TeamResult> results = [.. ByTeam.Values];
            results.Sort();
            results.Reverse();

            for (int i = 0; i < results.Count; i++) {
                TeamResult teamResult = results[i];
                teamResult.Rank = i + 1; // Rank starts at 1
            }
        }
    }
}
