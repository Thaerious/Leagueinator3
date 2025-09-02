using Leagueinator.GUI.Controllers.Modules;
using Leagueinator.GUI.Controllers.Modules.ScoringBowls;
using Leagueinator.GUI.Controllers.Modules.ScoringPlus;

namespace Leagueinator.GUI.Model.Enums {
    public enum MatchScoring {
        Bowls, Plus
    }

    public static class MatchScoringMeta {
        public static IModule GetModule(this MatchScoring matchScoring) {
            return matchScoring switch {
                MatchScoring.Bowls => new ScoringModule<BowlsResult>(),
                MatchScoring.Plus => new ScoringModule<PlusResult>(),
                _ => throw new NotImplementedException(),
            };
        }

        public static Type GetResultType(this MatchScoring matchScoring) {
            return matchScoring switch {
                MatchScoring.Bowls => typeof(BowlsResult),
                MatchScoring.Plus => typeof(PlusResult),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
