using Leagueinator.GUI.Modules.RankedLadder;
using Leagueinator.GUI.Modules;

namespace Leagueinator.GUI.Model {
    public enum EventType { RankedLadder, RoundRobin, Motley };

    public static class EventTypeMeta {
        public static IModule GetModule(EventType eventType) {
            return eventType switch {
                EventType.RankedLadder => new RankedLadderModule(),
            };
        }
    }
}

