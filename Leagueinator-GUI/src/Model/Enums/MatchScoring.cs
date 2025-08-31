using Leagueinator.GUI.Controllers.Modules;
using Leagueinator.GUI.Controllers.Modules.ScoringPlus;

namespace Leagueinator.GUI.Model.Enums {
    public enum MatchScoring {
        Bowls, Plus
    }

    public static class MatchScoringMeta {
        public static IModule GetModule(this MatchScoring matchScoring) {
            return matchScoring switch {
                //MatchScoring.Bowls => new ScoringBowlsModule(),
                MatchScoring.Plus => new ScoringPlusModule(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
