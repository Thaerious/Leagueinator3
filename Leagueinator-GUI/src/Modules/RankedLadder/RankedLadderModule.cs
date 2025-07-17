using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Modules.RankedLadder {
    public class RankedLadderModule : IModule {
        public void LoadHooks() {
            Hooks.GenerateRound = eventData => {
                RankedLadderImpl rankedLadder = new(eventData);
                return rankedLadder.GenerateRound();
            };  
        }

        public void LoadMenu() {
            // do nothing
        }
    }
}
