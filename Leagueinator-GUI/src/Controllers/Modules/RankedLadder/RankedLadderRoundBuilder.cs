using Leagueinator.GUI.Model;
using System.Diagnostics;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.RankedLadder {
    internal class RankedLadderRoundBuilder(EventData eventData) {
        public EventData EventData { get; } = eventData;

        /// <summary>
        /// Generates a new round based on the scores of the previous _rounds.
        /// Ignores any match that des not have any scores or bowls.
        /// </summary>
        /// <returns></returns>
        public RoundData GenerateRound() {
            RoundData newRound = new(this.EventData);
            var scores = RankedLadderModule.EventScores(this.EventData);

            while (scores.Count > 0) {
                MatchData matchData = new(newRound) {
                    MatchFormat = this.EventData.DefaultMatchFormat,
                    Ends = this.EventData.DefaultEnds,
                };

                TeamData bestTeam = scores[0].Team;
                scores.RemoveAt(0);

                int opponentIndex = this.GetOpponents(bestTeam, scores);
                TeamData opponent = scores[opponentIndex].Team;
                scores.RemoveAt(opponentIndex);

                matchData.Teams[0].CopyFrom(bestTeam);
                matchData.Teams[1].CopyFrom(opponent);
                newRound.AddMatch(matchData);

            }

            newRound.Fill();
            return newRound;
        }

        /// <summary>
        /// From the list of scores return the next team that has not played against 'team'.
        /// </summary>
        /// <param name="scores"></param>
        /// <returns></returns>
        private int GetOpponents(TeamData target, List<(TeamData Team, List<RoundResult> List, RoundResult Sum)> scores) {
            //this.EventData.AllTeams().Select(t => t.
            
            //var blackList = this.EventData.PreviousOpponents(target.AllNames());

            //Debug.WriteLine($"target {target.AllNames().JoinString()}");
            //Debug.WriteLine($"blacklist {blackList.JoinString()}");

            //for (int i = 0; i < scores.Count; i++) {
            //    bool hasPlayed = scores[i].Team.GetOpposition().AllNames().Intersect(blackList).Any();
            //    if (!hasPlayed) return i;
            //}

            throw new UnpairableTeamsException(target.AllNames());
        }
    }
}
