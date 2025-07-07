using Leagueinator.GUI.Utility;

namespace Leagueinator.GUI.Controllers.NamedEvents { 

    public class ArgTable : Dictionary<string, object> { }

    public class NamedEventArgs : EventArgs {
        public NamedEventArgs(EventName eventName, ArgTable data) {
            this.EventName = eventName;
            this.Data = data;
            this.Trace = Logger.GetInvocationLoc();
        }

        public NamedEventArgs(EventName eventName) {
            this.EventName = eventName;
            this.Data = [];
            this.Trace = Logger.GetInvocationLoc();
        }

        #region Properties
        public EventName EventName { get; }

        public bool Handled { get; set; } = false;

        public ArgTable Data { get; }

        public string Trace { get; private set; }   

        #endregion
    }
}
