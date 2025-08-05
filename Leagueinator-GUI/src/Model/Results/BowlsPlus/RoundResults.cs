using System.Diagnostics;

namespace Leagueinator.GUI.Model.Results.BowlsPlus {
    /// <summary>
    /// Represents the results for all teams within a single round.
    /// </summary>
    public class RoundResults {

        /// <summary>
        /// Gets the list of individual team results for the round.
        /// The results are sorted in descending order of performance, with ranks assigned.
        /// </summary>
        public List<SingleResult> AllResults { get; private set; } = [];

        public List<MatchResult> ByMatch { get; private set; } = [];

        /// <summary>
        /// Constructs a <see cref="RoundResults"/> object from the provided <see cref="RoundData"/>.
        /// Calculates results for each team in each match, assigns ranks based on performance.
        /// </summary>
        /// <param name="RoundData">The round data containing matches and teams.</param>
        public RoundResults(RoundData RoundData) {
            List<SingleResult> results = [];

            foreach (MatchData matchData in RoundData.Matches) {
                if (matchData.CountPlayers() == 0) continue;

                MatchResult matchResult = new() {Lane = matchData.Lane };

                for (int team = 0; team < matchData.Teams.Count; team++) {
                    SingleResult result = new(matchData, team);
                    results.Add(result);
                    matchResult.Add(result);
                }
                this.ByMatch.Add(matchResult);
            }

            results.Sort();
            this.AllResults = [.. results.AsEnumerable()];

            for (int i = 0; i < this.AllResults.Count; i++) {
                SingleResult result = this.AllResults.ElementAt(i);
                result.Rank = i + 1; // Rank starts at 1
            }
        }
    }
}
