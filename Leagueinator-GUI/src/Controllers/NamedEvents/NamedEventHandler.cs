namespace Leagueinator.GUI.Controllers.NamedEvents {
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NamedEventHandler : Attribute {
        public EventName EventName { get; }

        public NamedEventHandler(EventName eventName) {
            this.EventName = eventName;
        }
    }
}
