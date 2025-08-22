
namespace Leagueinator.GUI.Model.Results.BowlsPlus {
    public class MatchResult : List<SingleResult>, IMatchResult {

        public int Lane { get; init; }

        public Players GetWinners() {
            throw new NotImplementedException();
            //foreach (SingleResult singleResult in this) {
            //    if (singleResult.GameResult == GameResult.Win) {
            //        return singleResult.AllNames;
            //    }
            //}

            //foreach (SingleResult singleResult in this) {
            //    Debug.WriteLine(singleResult);
            //}
            //throw new ModuleException("Match must have a winner to calculate ELODictionary");
        }

        public Players GetLosers() {
            throw new NotImplementedException();
            //foreach (SingleResult singleResult in this) {
            //    if (singleResult.GameResult == GameResult.Loss) {
            //        return singleResult.AllNames;
            //    }
            //}
            //throw new ModuleException("Match must have a loser to calculate ELODictionary");
        }
    }
}
