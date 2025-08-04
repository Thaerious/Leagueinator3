namespace Leagueinator.GUI.Model {
    public enum EventType { RankedLadder, Motley };

    public static class EventTypeMeta {
        // This should be part of the module package TODO
        //public static IModule GetModule(EventType eventType) {
        //    return eventType switch {
        //        EventType.RankedLadder => new RankedLadderModule(),
        //        EventType.Motley => new MotleyModule(),
        //    };
        //}
    }
}

