using Leagueinator.GUI.src.Controllers;
using Leagueinator.GUI.Utility;

namespace Leagueinator.GUI.Forms.Main {

    public class DataTable : Dictionary<string, object> { }

    public class NamedEventArgs : EventArgs {
        public NamedEventArgs(EventName eventName, DataTable data) {
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

        public DataTable Data { get; }

        public String Trace { get; private set; }   

        #endregion
    }
}
