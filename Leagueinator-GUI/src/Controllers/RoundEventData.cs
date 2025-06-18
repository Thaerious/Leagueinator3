using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controllers {
    public class RoundEventData(string Action, int index, RoundData? roundData) {
        public string Action { get; } = Action;

        public int Index { get; } = index;

        public RoundData? RoundData { get; } = roundData;        
    }
}
