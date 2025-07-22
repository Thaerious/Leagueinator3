using Leagueinator.GUI.Utility;

namespace Leagueinator.GUI.Controllers.NamedEvents {

    public class ArgTable : Dictionary<string, object> {
        public static readonly ArgTable Empty = [];
    }

    public class NamedEventArgs : EventArgs {
        public NamedEventArgs(EventName eventName, object source, ArgTable data) {
            this.EventName = eventName;
            this.Data = data;
            this.Source = source;
            this.Trace = Logger.GetInvocationLoc();
        }

        public NamedEventArgs(EventName eventName, object source) {
            this.EventName = eventName;
            this.Data = [];
            this.Source = source;
            this.Trace = Logger.GetInvocationLoc();
        }

        #region Properties
        public EventName EventName { get; }

        public bool Handled { get; set; } = false;

        public ArgTable Data { get; }

        public object Source { get; }
        public string Trace { get; private set; }   

        #endregion
    }
}
