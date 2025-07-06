namespace Leagueinator.GUI.Model {
    public class EventData {

        public string EventName { get; set; } = "Name Not Set";

        public DateTime Date { get; set; } = DateTime.Now;

        public MatchFormat MatchFormat { get; set; } = MatchFormat.VS2;
        public int LaneCount { get; set; } = 8;
        public int DefaultEnds { get; set; } = 10;
        public EventType EventType { get; set; } = EventType.RankedLadder;

        public RoundDataCollection Rounds { get; } = [];

        required public int UID { get; set; }
    }
}
