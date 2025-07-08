

namespace Leagueinator.GUI.Model {
    public class LeagueData : List<EventData> {

        private int NextUID = 1;
        public int GetNextUID() {
            return this.NextUID++;
        }

        public EventData GetEvent(int eventUID) {
            foreach (EventData @event in this) {
                if (@event.UID == eventUID) return @event;
            }
            throw new KeyNotFoundException();
        }

        internal EventData AddEvent() {
            EventData eventData = new() {
                UID = this.GetNextUID()
            };
            this.Add(eventData);
            return eventData;
        }
    }
}
