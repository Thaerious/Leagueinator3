namespace Leagueinator.GUI.Model {
    public class ReadOnlyEventData {

        private readonly EventData EventData;

        public List<ReadOnlyRoundData> Rounds { get; } = [];

        public ReadOnlyEventData(EventData eventData) {
            this.EventData = eventData.Copy();

            foreach (RoundData roundData in eventData.Rounds) {
                this.Rounds.Add(roundData.AsReadOnly());
            }
        }

        public string EventName => EventData.EventName;
        public DateTime Date => EventData.Date;
        public MatchFormat MatchFormat => EventData.MatchFormat;
        public int LaneCount => EventData.LaneCount;
        public int DefaultEnds => EventData.DefaultEnds;
        public EventType EventType => EventData.EventType;
        public int UID => EventData.UID;
    }

    public class EventData {

        public string EventName { get; set; } = "Name Not Set";

        public DateTime Date { get; set; } = DateTime.Now;

        public MatchFormat MatchFormat { get; set; } = MatchFormat.VS2;
        public int LaneCount { get; set; } = 8;
        public int DefaultEnds { get; set; } = 10;
        public EventType EventType { get; set; } = EventType.RankedLadder;

        public RoundDataCollection Rounds { get; } = [];

        required public int UID { get; set; }

        public EventData Copy() {
            var copy = new EventData {
                EventName = this.EventName,
                Date = this.Date,
                MatchFormat = this.MatchFormat,
                LaneCount = this.LaneCount,
                DefaultEnds = this.DefaultEnds,
                EventType = this.EventType,
                UID = this.UID
            };

            foreach (var round in this.Rounds) {
                copy.Rounds.Add(round.Copy()); // assumes Rounds.Copy() already returns a deep copy
            }

            return copy;
        }

        public ReadOnlyEventData AsReadOnly() {
            return new(this);
        }
    }
}
