namespace Leagueinator.GUI.Model {
    public enum MatchFormat { VS1, VS2, VS3, VS4, A4321 };

    public static class MatchFormatMeta {
        public static string ToString(this MatchFormat matchFormat) {
            switch (matchFormat) {
                case MatchFormat.VS1:   return "1 vs 1";
                case MatchFormat.VS2:   return "2 vs 2";
                case MatchFormat.VS3:   return "3 vs 3";
                case MatchFormat.VS4:   return "4 vs 4";
                case MatchFormat.A4321: return "4321";
                default: throw new KeyNotFoundException();
            }
        }

        public static int TeamSize(this MatchFormat matchFormat) {
            switch (matchFormat) {
                case MatchFormat.VS1: return 1;
                case MatchFormat.VS2: return 2;
                case MatchFormat.VS3: return 3;
                case MatchFormat.VS4: return 4;
                case MatchFormat.A4321: return 1;
                default: throw new KeyNotFoundException();
            }
        }

        public static int TeamCount(this MatchFormat matchFormat) {
            switch (matchFormat) {
                case MatchFormat.VS1: return 2;
                case MatchFormat.VS2: return 2;
                case MatchFormat.VS3: return 2;
                case MatchFormat.VS4: return 2;
                case MatchFormat.A4321: return 4;
                default: throw new KeyNotFoundException();
            }
        }
    }
}

