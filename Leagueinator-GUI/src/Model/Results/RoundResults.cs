using System.Diagnostics;

namespace Leagueinator.GUI.Model.Results {
    /// <summary>
    /// Represents the results for all teams within a single round.
    /// </summary>
    internal class RoundResults {

        /// <summary>
        /// Gets the list of individual team results for the round.
        /// The results are sorted in descending order of performance, with ranks assigned.
        /// </summary>
        public List<SingleResult> Results { get; private set; } = [];

        /// <summary>
        /// Constructs a <see cref="RoundResults"/> object from the provided <see cref="RoundData"/>.
        /// Calculates results for each team in each match, assigns ranks based on performance.
        /// </summary>
        /// <param name="RoundData">The round data containing matches and teams.</param>
        public RoundResults(RoundData RoundData) {
            List<SingleResult> results = [];

            foreach (MatchData matchData in RoundData) {
                if (matchData.CountPlayers() == 0) continue;

                for (int team = 0; team < matchData.Teams.Length; team++) {
                    SingleResult result = new(matchData, team);
                    results.Add(result);
                }
            }

            results.Sort();
            this.Results = [.. results.AsEnumerable().Reverse()];

            for (int i = 0; i < this.Results.Count; i++) {
                SingleResult result = this.Results.ElementAt(i);
                result.Rank = i + 1; // Rank starts at 1
            }
        }
    }
}
