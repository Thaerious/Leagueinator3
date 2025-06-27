using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;

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

            while (results.Count > 0) {
                TeamData bestTeam = results[0].Team;
                results.RemoveAt(0);
                TeamResult nextResult = this.GetNextTeam(bestTeam, results);
                results.Remove(nextResult);

                MatchData matchData = new(this.EventData.MatchFormat) {
                    Lane = newRound.Count,
                    Ends = this.EventData.DefaultEnds,
                };

                matchData.AddTeam(bestTeam.Copy()); 
                matchData.AddTeam(nextResult.Team.Copy()); 
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
        private TeamResult GetNextTeam(TeamData team, List<TeamResult> results) {
            foreach (TeamResult result in results) {
                if (this.RoundDataCollection.HasPlayed(team, result.Team)) continue;
                return result; 
            }

            throw new UnpairableTeamsException(team);
        }
    }
}
