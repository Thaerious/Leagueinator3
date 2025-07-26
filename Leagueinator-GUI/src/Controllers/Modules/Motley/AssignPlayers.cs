using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    public class AssignPlayers {

        List<string> setAside = [];
        private EventData EventData;
        Dictionary<string, int> elo = [];

        public void Balanced(LeagueData leagueData, EventData eventData, RoundData roundData) {
            this.EventData = eventData;
            this.elo = ELO.CalculateELO(leagueData);
            

            if (elo.Count % 2 != 0) {
                this.SetAside();
            }
            


        }

        private void SetAside() {
            //this.elo.OrderBy(kv => kv.Value)
            //    .Where(kv => this.EventData.

            //List<KeyValuePair<string, int>> sortedElo = [.. this.elo.OrderBy(kv => kv.Value)];
            
        }
    }
}
