
using Leagueinator.GUI.Modules;

namespace Leagueinator.GUI.Model.Results {
    public class MatchResult : List<SingleResult> {
        internal Players GetWinners() {
            foreach (SingleResult singleResult in this) {
                if (singleResult.Result == Result.Win) {
                    return singleResult.Players;
                }
            }
            throw new ModuleException("Match must have a winner to calculate ELO");
        }

        internal Players GetLosers() {
            foreach (SingleResult singleResult in this) {
                if (singleResult.Result == Result.Loss) {
                    return singleResult.Players;
                }
            }
            throw new ModuleException("Match must have a loser to calculate ELO");
        }
    }
}
