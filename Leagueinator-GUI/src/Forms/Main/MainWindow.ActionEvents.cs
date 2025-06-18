using Leagueinator.GUI.Model;
using System.Windows;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {
        public class ActionEventArg(string action, object? data) : EventArgs {
            public string Action { get; private set; } = action;
            public object? Data { get; private set; } = data;
        }

        public event EventHandler<ActionEventArg>? OnActionEvent;

        public void InvokeActionEvent(string action, object? data = null) {
            this.OnActionEvent?.Invoke(this, new ActionEventArg(action, data));
        }
    }
}
