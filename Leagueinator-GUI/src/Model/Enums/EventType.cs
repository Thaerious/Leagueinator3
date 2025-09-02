using Leagueinator.GUI.Controllers.Modules;
using Leagueinator.GUI.Controllers.Modules.Motley;
using Leagueinator.GUI.Controllers.Modules.Swiss;

namespace Leagueinator.GUI.Model.Enums {
    public enum EventType { Swiss, Jitney };

    public static class EventTypeMeta {
        public static IModule GetModule(this EventType eventType) {
            return eventType switch {
                EventType.Swiss => new SwissModule(),
                EventType.Jitney => new MotleyModule(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}

