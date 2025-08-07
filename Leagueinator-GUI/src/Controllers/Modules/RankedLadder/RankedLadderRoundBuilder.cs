using Leagueinator.GUI.Model;
using System.Diagnostics;
using Utility;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.RankedLadder {
    internal class RankedLadderRoundBuilder(EventData eventData) {
        public EventData EventData { get; } = eventData;

        /// <summary>
        /// Generates a new round based on the players of the previous _rounds.
        /// Ignores any match that des not have any players or bowls.
        /// </summary>
        /// <returns></returns>
        public RoundData GenerateRound() {
            RoundData newRound = new(this.EventData);
            DefaultDictionary<Players, PlusResultsSchema> summary = [];

            foreach (TeamData team in this.EventData.AllTeams()) {
                if (team.CountPlayers() == 0) continue;
                var key = new Players(team.Names);            // ensure Players uses set-like equality
                summary[key] += new PlusResultsSchema(team);  // uses operator +
            }

            var players = summary.OrderBy(kvp => kvp.Value)
                                 .Select(kvp => kvp.Key)
                                 .ToList();

            while (players.Count > 0) {
                Players bestTeam = players[0];
                players.RemoveAt(0);

                Players opponents = this.GetOpponents(bestTeam, players);
                players.Remove(opponents);

                MatchData matchData = new(newRound) {
                    MatchFormat = this.EventData.DefaultMatchFormat,
                    Ends = this.EventData.DefaultEnds,
                };

                // TODO Generalize for 4321
                matchData.Teams[0].CopyFrom(bestTeam);
                matchData.Teams[1].CopyFrom(opponents);
                newRound.AddMatch(matchData);
            }

            newRound.Fill();
            return newRound;
        }

        /// <summary>
        /// From the list of players return the next team that has not played against 'team'.
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        private Players GetOpponents(Players target, List<Players> players) {
            Debug.WriteLine($"Finding opponents for {target.JoinString()}");
            var previousOpponents = this.PreviousOpponents(target);

            foreach (Players opponents in players) {
                if (previousOpponents.Contains(opponents)) continue;
                return opponents;
            }

            throw new UnpairableTeamsException(target);
        }

        private HashSet<Players> PreviousOpponents(Players target) {
            HashSet<Players> previous = [];

            foreach (TeamData teamData in this.EventData.AllTeams()) {
                if (!teamData.Equals(target)) continue;
                foreach (TeamData opponents in teamData.Parent.Teams) {
                    previous.Add([.. opponents.Names]);
                }
            }
            foreach (Players x in previous) Debug.WriteLine(x.JoinString());
            return previous;
        }
    }
}
