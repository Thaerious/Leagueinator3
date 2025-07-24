using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;

namespace Leagueinator.GUI.Model {
    public class EventDataStats {
        private EventData EventData;

        public EventDataStats(EventData eventData) {
            this.EventData = eventData;
        }

        public HashSet<string> PreviousPartners(string name) {
            HashSet<string> previous = [];

            foreach (TeamData team in this.EventData.Teams) {                
                if (team.Names.Contains(name)) {
                    team.Names.Where(n => n != name).ToList().ForEach(n => previous.Add(n));
                }
            }

            return previous;
        }

    }
}
