namespace Leagueinator.GUI.Model {
    public enum MatchFormat { VS1, VS2, VS3, VS4, A4321 };

    public static class MatchFormatMeta {
        public static readonly Dictionary<MatchFormat, (int TeamCount, int TeamSize, string Label)> Info = new() {
            { MatchFormat.VS1, (2, 1, "1 vs 1") },
            { MatchFormat.VS2, (2, 2, "2 vs 2") },
            { MatchFormat.VS3, (2, 3, "3 vs 3") },
            { MatchFormat.VS4, (2, 4, "4 vs 4") },
            { MatchFormat.A4321, (4, 1, "4321") },
        };
    }
}

