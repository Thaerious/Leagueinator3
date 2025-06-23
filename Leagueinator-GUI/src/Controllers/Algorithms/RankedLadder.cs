using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;
using System.Diagnostics;

namespace Leagueinator.GUI.Controllers.Algorithms {
    internal class RankedLadder {
        public RoundDataCollection RoundDataCollection { get; }
        public EventData EventData { get; }

        public RankedLadder(RoundDataCollection roundDataCollection, EventData eventData) {
            this.RoundDataCollection = roundDataCollection;
            this.EventData = eventData;
        }

        /// <summary>
        /// Generates a new round based on the results of the previous rounds.
        /// Ignores any match that des not have any players or bowls.
        /// </summary>
        /// <returns></returns>
        public RoundData GenerateRound() {
            RoundData newRound = new RoundData();
            List<TeamResult> results = new EventResults(this.RoundDataCollection).ResultsByTeam;
            int next = 0;

            while (results.Count > 0) {
                TeamResult best = results[0];
                results.RemoveAt(0);
                var nextTeam = this.GetNextTeam(best.Team, results);
                results.RemoveAll(r => r.Team.SequenceEqual(nextTeam));

                MatchData matchData = new(this.EventData.MatchFormat) {
                    Lane = newRound.Count,
                    Ends = this.EventData.DefaultEnds,
                    Teams = [best.Team, nextTeam]
                };

                newRound.Add(matchData);
            }

            newRound.Fill(this.EventData);
            return newRound;
        }

        /// <summary>
        /// From the list of results return the next team that has not played against 'team'.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private string[] GetNextTeam(string[] team, List<TeamResult> results) {
            foreach (TeamResult result in results) {
                if (this.RoundDataCollection.HasPlayed(team, result.Team)) continue;
                return result.Team; // Found a team that has not played against 'team'
            }
            
            throw new PreconditionException("No team found that has not played against the given team.");
        }
    }
}
