using Leagueinator.GUI.Model;
using System.Windows;

namespace Leagueinator.GUI.Modules.RankedLadder {
    public class RankedLadderModule : IModule {

        public void LoadModule(Window window) {
            Hooks.GenerateRound = this.GenerateRound;
        }

        public void UnloadModule(Window window) {
            if (Hooks.GenerateRound == this.GenerateRound) {
                Hooks.GenerateRound = null;
            }
        }

        public RoundData GenerateRound(EventData eventData) {
            RankedLadderImpl rankedLadder = new(eventData);
            return rankedLadder.GenerateRound();
        }
    }
}
