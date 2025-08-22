using Algorithms.Binning;
using Leagueinator.GUI.Controllers.Modules.ELO;
using Leagueinator.GUI.Model;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.ELO {

    public class BinSeedPlayers {
        private EventData EventData;
        private LeagueData LeagueData { get; }
        private RoundData RoundData { get; }
        private ELODictionary ELO { get; init; }

        public BinSeedPlayers(RoundData roundData) {
            this.RoundData = roundData;
            this.EventData = roundData.Parent;
            this.LeagueData = this.EventData.Parent;
            this.ELO = new(this.LeagueData);
        }

        public RoundData NewRound() {
            List<int> binSizes = [];

            foreach (MatchData matchData in this.RoundData.Matches) {
                foreach (TeamData teamData in matchData.Teams) {
                    if (teamData.IsEmpty()) continue;
                    binSizes.Add(teamData.Names.Count);
                }
            }

            BestFirst<string> algorithm = new(this.ELO, binSizes);
            var results = algorithm.Solve();

            RoundData newRound = new(this.EventData);

            foreach (MatchData matchData in this.RoundData.Matches) {
                MatchData newMatch = new(newRound) {
                    MatchFormat = matchData.MatchFormat,
                    Ends = matchData.Ends,
                };

                newRound.AddMatch(newMatch);

                foreach (TeamData teamData in matchData.Teams) {
                    if (teamData.IsEmpty()) continue;
                    TeamData newTeam = newMatch.Teams[teamData.Index];

                    IReadOnlyList<string> result = results.Dequeue();
                    foreach (string name in result) {
                        newTeam.AddPlayer(name);
                    }
                }
            }

            return newRound;
        }
    }
}
