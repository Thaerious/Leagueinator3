using Leagueinator.GUI.Controllers.Modules;
using Leagueinator.GUI.Controllers.Modules.Motley;
using Leagueinator.GUI.Controllers.Modules.RankedLadder;

namespace Leagueinator.GUI.Model.Enums {
    public enum EventType { RankedLadder, Jitney };

    public static class EventTypeMeta {
        public static IModule GetModule(this EventType eventType) {
            return eventType switch {
                EventType.RankedLadder => new RankedLadderModule(),
                EventType.Jitney => new MotleyModule(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}

