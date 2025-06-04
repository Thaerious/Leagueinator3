using Leagueinator.GUI.Utility;
using System.Diagnostics;

namespace Leagueinator.GUI.Forms.Main {

    public record PlayerData(string Name, int Team);

    public class MatchData {
        public MatchFormat MatchFormat { get; set; }
        public int Lane { get; set; } = -1;
        public readonly int[] Score = new int[4];
        public readonly List<PlayerData> Players = [];
        public int TieBreaker { get; set; } = -1;
    }

    public class RoundData : List<MatchData> {
        public RoundData(MatchFormat matchFormat, int LaneCount) {
            while (this.Count < LaneCount) {
                this.Add(new MatchData {
                    MatchFormat = matchFormat,
                    Lane = this.Count + 1
                });
            }
        }

        public readonly Dictionary<int, MatchData> Matches = [];
    }

    public class EventData {
        public MatchFormat MatchFormat { get; set; } = MatchFormat.VS2;
        public int LaneCount { get; set; } = 8;
    }
}
