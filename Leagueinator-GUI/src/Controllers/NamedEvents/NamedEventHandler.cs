namespace Leagueinator.GUI.Controllers.NamedEvents {
    public class NamedEventHandler : Attribute {
        public EventName EventName { get; }

        public NamedEventHandler(EventName eventName) {
            this.EventName = eventName;
        }
    }
}
