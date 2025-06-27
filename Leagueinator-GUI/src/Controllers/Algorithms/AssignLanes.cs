using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leagueinator.GUI.src.Controllers.Algorithms {
    public class AssignLanes {
        private EventData EventData { get; }
        private RoundDataCollection Rounds { get; }
        private RoundData TargetRound { get; }

        private Dictionary<TeamData, HashSet<int>> TeamHistory = [];

        private Dictionary<MatchData, HashSet<int>> MatchHistory = [];

        public Dictionary<MatchData, int> LaneAssignments { get; } = [];

        public AssignLanes(EventData eventData, RoundDataCollection rounds, RoundData target) {
            this.EventData = eventData;
            this.Rounds = rounds;
            this.TargetRound = target;
        }

        public RoundData DoAssignment() {
            this.BuildTeamHistory();
            this.BuildMatchHistory();
            this.InvertMatchHistory();

            //foreach (MatchData match in this.TargetRound) {
            //    if (match.CountPlayers() == 0) continue;
            //    Debug.WriteLine($"Assigning lanes for match {match}");
            //    Debug.WriteLine($"Lane History: {string.Join(", ", this.MatchHistory[match])}");
            //}

            return this.DoAssignLanes();
        }


        private RoundData DoAssignLanes() {
            RoundData newRound = [];
            HashSet<int> usedLanes = [];

            foreach (MatchData match in this.MatchHistory.Keys) {
                int lane = this.MatchHistory[match].First(l => !usedLanes.Contains(l));
                usedLanes.Add(lane);

                MatchData newMatch = new MatchData(match.MatchFormat) {
                    Teams = [.. match.Teams.Select(t => t.Copy())],
                    Lane = lane,
                    Ends = this.EventData.DefaultEnds,
                };

                newRound.Add(newMatch);
                newRound.Sort((a, b) => a.Lane.CompareTo(b.Lane));
            }

            newRound.Fill(this.EventData);  
            return newRound;
        }

        private void InvertMatchHistory() {
            foreach (MatchData match in this.MatchHistory.Keys) {
                HashSet<int> inverse = Enumerable.Range(0, this.EventData.LaneCount).ToHashSet();
                inverse.ExceptWith(this.MatchHistory[match]);
                this.MatchHistory[match] = inverse;
            }
        }

        private void BuildMatchHistory() {
            foreach (MatchData match in this.TargetRound) {
                if (match.CountPlayers() == 0) continue;
                this.MatchHistory[match] = [];

                foreach (TeamData team in match.Teams) {
                    this.MatchHistory[match].UnionWith(this.TeamHistory[team]);
                }
            }
        }

        /// <summary>
        /// Update the specified RoundData with the assigned lanes.
        /// Lanes are assigned so that lanes do not repeat from previous rounds.
        /// Changes are applied to the provided target.
        /// Current lane assignments from target are not considered.
        /// </summary>
        /// <param name="rounds"></param>
        /// <param name="roundData"></param>
        public void BuildTeamHistory() {
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
        }
    }
}
