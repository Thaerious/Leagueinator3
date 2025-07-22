namespace Leagueinator.GUI.Model.Results {
    internal interface IMatchResult {

        public int Lane { get; init; }
        public Players GetWinners();
        public Players GetLosers();
    }
}
