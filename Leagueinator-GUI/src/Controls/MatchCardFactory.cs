using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controls {
    public static class MatchCardFactory {
        public static MatchCard GenerateMatchCard(MatchFormat matchFormat) {
            switch (matchFormat) {
                case MatchFormat.VS1:
                    return new MatchCardV1();
                case MatchFormat.VS2:
                    return new MatchCardV2();
                case MatchFormat.VS3:
                    return new MatchCardV3();
                case MatchFormat.VS4:
                    return new MatchCardV4();
                case MatchFormat.A4321:
                    return new MatchCard4321();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
