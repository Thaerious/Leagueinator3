namespace Leagueinator.GUI.Model {

    public class EventData {

        public string EventName { get; set; } = "Name Not Set";

        public DateTime Date { get; set; } = DateTime.Now;

        public MatchFormat MatchFormat { get; set; } = MatchFormat.VS2;
        public int LaneCount { get; set; } = 8;
        public int DefaultEnds { get; set; } = 10;
        public EventType EventType { get; set; } = EventType.RankedLadder;

        public RoundDataCollection Rounds { get; set; } = [];

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
    }
}
