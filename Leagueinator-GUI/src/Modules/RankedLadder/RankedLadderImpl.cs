using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;
using System.Diagnostics;

namespace Leagueinator.GUI.Modules.RankedLadder {
    internal class RankedLadderImpl {
        public EventData EventData { get; }

        public RankedLadderImpl(EventData eventData) {
            this.EventData = eventData;
        }

        /// <summary>
        /// Generates a new round based on the results of the previous rounds.
        /// Ignores any match that des not have any players or bowls.
        /// </summary>
        /// <returns></returns>
        public RoundData GenerateRound() {
            RoundData newRound = [];
            List<TeamResult> results = new EventResults(this.EventData).ResultsByTeam;

            foreach (TeamResult result in results) {
                Debug.WriteLine(result);
            }

            while (results.Count > 0) {
                Players bestTeam = results[0].Players;
                results.RemoveAt(0);
                TeamResult nextResult = this.GetNextTeam(bestTeam, results);
                results.Remove(nextResult);

                MatchData matchData = new(this.EventData.MatchFormat) {
                    Lane = newRound.Count,
                    Ends = this.EventData.DefaultEnds,
                };

                matchData.AddTeam(bestTeam); 
                matchData.AddTeam(nextResult.Players); 
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
        private TeamResult GetNextTeam(IEnumerable<string> players, List<TeamResult> results) {
            foreach (TeamResult result in results) {
                if (this.EventData.HasPlayed(players, result.Players)) continue;
                return result; 
            }

            throw new UnpairableTeamsException(players);
        }
    }
}
