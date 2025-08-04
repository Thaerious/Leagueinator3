using Leagueinator.GUI.Model.ViewModel;
using Utility.Extensions;

namespace Leagueinator.GUI.Model {
    public class LeagueData : List<EventData> {

        private readonly List<EventData> events = [];
        public IReadOnlyList<EventData> Events => events;

        internal EventData AddEvent(string? name = null) {
            name = name ?? DateTime.Now.ToString("MMMM d, yyyy");
            int count = this.Count(e => e.EventName.StartsWith(name));
            if (count > 0) name = $"{name} [{count}]";

            EventData eventData = new(this) {
                EventName = name
            };

            this.Add(eventData);
            return eventData;
        }

        public override string ToString() {
            string sb = $"{this.Count}\n";

            foreach (EventData @event in this) {
                sb += EventData.ToRecord(@event).ToString() + "\n";

                foreach (RoundData round in @event) {
                    var matchRecords = MatchRecord.MatchRecordList(round);
                    var records = round.Records();

                    sb += $"{matchRecords.Count}\n";
                    foreach (MatchRecord matchRecord in matchRecords) {
                        sb += matchRecord.ToString() + "\n";
                    }

                    sb += $"{records.Count()}\n";
                    foreach (PlayerRecord roundRecord in records) {
                        sb += roundRecord.ToString() + "\n";
                    }
                }
            }

            return sb;
        }

        public IEnumerable<PlayerRecord> Records() {
            return this.Events.SelectMany(round => round.Records());
        }
    }
}
