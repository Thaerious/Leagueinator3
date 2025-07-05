using Leagueinator.GUI.src.Controllers;
using Leagueinator.GUI.Utility;
using System;
using System.Diagnostics;
using System.Windows;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {

        public event EventHandler<NamedEventArgs> OnNamedEvent = delegate { };

        public void InvokeNamedEvent(EventName eventName) {
            Debug.WriteLine($"InvokeNamedEvent w/o data {eventName}");
            NamedEventArgs args = new(eventName);
            this.OnNamedEvent.Invoke(this, args);

            if (args.Handled == false) {
                Logger.Log($"Warning: Event '{eventName}' not handled.");
            }
        }

        public void InvokeNamedEvent(EventName eventName, DataTable data) {
            Debug.WriteLine($"InvokeNamedEvent w/data {eventName}");
            NamedEventArgs args = new(eventName, data);
            this.OnNamedEvent.Invoke(this, args);

            if (args.Handled == false) {
                Logger.Log($"Warning: Event '{eventName}' not handled.");
            }
        }
    }
}
