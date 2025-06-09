using Leagueinator.GUI.Utility;

namespace Leagueinator.GUI.Forms.Main {
    public class EventData {
        public MatchFormat MatchFormat { get; set; } = MatchFormat.VS2;
        public int LaneCount { get; set; } = 8;
        public int DefaultEnds { get; set; } = 10;
    }
}
