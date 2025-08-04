
namespace Leagueinator.GUI.Model.Results.BowlsPlus {
    public class MatchResult : List<SingleResult>, IMatchResult {

        public int Lane { get; init; }

        public Players GetWinners() {
            throw new NotImplementedException();
            //foreach (SingleResult singleResult in this) {
            //    if (singleResult.Result == Result.Win) {
            //        return singleResult.Players;
            //    }
            //}

            //foreach (SingleResult singleResult in this) {
            //    Debug.WriteLine(singleResult);
            //}
            //throw new ModuleException("Match must have a winner to calculate ELO");
        }

        public Players GetLosers() {
            throw new NotImplementedException();
            //foreach (SingleResult singleResult in this) {
            //    if (singleResult.Result == Result.Loss) {
            //        return singleResult.Players;
            //    }
            //}
            //throw new ModuleException("Match must have a loser to calculate ELO");
        }
    }
}
