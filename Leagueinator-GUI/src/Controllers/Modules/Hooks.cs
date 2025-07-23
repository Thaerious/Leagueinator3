using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controllers.Modules {

    /// <summary>
    /// Functions that are used to hook into the controller and model.
    /// A module can set and replace behavior on the hooks w/o the controller needing to know.
    /// </summary>
    public static class Hooks {
        public delegate RoundData GenerateRoundDelegate(EventData eventData);
        public delegate RoundData ScoringDelegate(EventData eventData);

        public static GenerateRoundDelegate? GenerateRound = default;
    }
}
