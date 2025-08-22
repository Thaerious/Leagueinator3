using Algorithms.Binning;
using Leagueinator.GUI.Model;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.ELO {

    public class BinSeedPlayers {
        private EventData EventData;
        private RoundData RoundData { get; }
        private ELODictionary ELO { get; init; }

        public BinSeedPlayers(RoundData roundData) {
            this.RoundData = roundData;
            this.EventData = roundData.Parent;
            this.ELO = new(this.EventData);
        }

        public RoundData NewRound() {
            RoundData best = this.NewRoundIteration();

            for (int i = 0; i < 10; i++) {
                RoundData next = this.NewRoundIteration();
                if (next.Fitness() < best.Fitness()) best = next;
            }

            return best;
        }

        private RoundData NewRoundIteration() {
            List<int> binSizes = [];

            // Evaluate bin sizes
            foreach (MatchData matchData in this.RoundData.Matches) {
                foreach (TeamData teamData in matchData.Teams) {
                    if (teamData.IsEmpty()) continue;
                    binSizes.Add(teamData.ToPlayers().Count);
                }
            }

            // Assign names to the bins
            BestFirst<string> algorithm = new(this.ELO, binSizes);
            var results = algorithm.Solve();
            RoundData newRound = new(this.EventData);

            // Use the bins to build new matches
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
