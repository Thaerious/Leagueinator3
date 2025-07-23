using Leagueinator.GUI.Model;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.RankedLadder {
    public class RankedLadderModule : BaseModule {

        public override void LoadModule(Window window, LeagueData leagueData) {
            base.LoadModule(window, leagueData);
        }

        public override void UnloadModule() {
        }

        public RoundData GenerateRound(EventData eventData) {
            RankedLadderImpl rankedLadder = new(eventData);
            return rankedLadder.GenerateRound();
        }
    }
}
