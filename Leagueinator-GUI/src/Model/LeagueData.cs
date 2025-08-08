using Leagueinator.GUI.Model.ViewModel;
using System.Diagnostics;
using System.IO;

namespace Leagueinator.GUI.Model {
    public class LeagueData {

        private readonly List<EventData> _events = [];
        public IReadOnlyList<EventData> Events => _events;

        internal EventData AddEvent(string? name = null) {
            name = name ?? DateTime.Now.ToString("MMMM dd yyyy");
            int count = this._events.Count(e => e.EventName.StartsWith(name));
            if (count > 0) name = $"{name} [{count}]";

            EventData eventData;

            if (this.Events.Count > 0) {
                eventData = new(this) {
                    EventName = name,
                    DefaultMatchFormat = this.Events[^1].DefaultMatchFormat,
                    EventType = this.Events[^1].EventType,
                    DefaultEnds = this.Events[^1].DefaultEnds,
                    LaneCount = this.Events[^1].LaneCount,
                };
            }
            else {
                eventData = new(this) {
                    EventName = name
                };
            }

            this._events.Add(eventData);
            return eventData;
        }

        internal void RemoveEvent(EventData eventData) {
            this._events.Remove(eventData);
        }

        public IEnumerable<PlayerRecord> Records() {
            return this.Events.SelectMany(round => round.Records());
        }

        public void WriteOut(StreamWriter writer) {
            writer.WriteLine(this.Events.Count);

            foreach (EventData @event in this._events) {
                @event.WriteOut(writer);
            }
        }
        
        public static LeagueData ReadIn(StreamReader reader) {
            LeagueData leagueData = new();
            int eventCount = int.Parse(reader?.ReadLine() ?? throw new FormatException("Invalid save format"));
            for (int i = 0; i < eventCount; i++) {
                EventData eventData = EventData.ReadIn(leagueData, reader);
                leagueData._events.Add(eventData);
            }
            return leagueData;
        }
    }
}
