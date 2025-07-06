using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.src.Controllers;
using Leagueinator.GUI.Utility;
using System.Diagnostics;
using System.Windows;

namespace Leagueinator.GUI.Forms.Event {
    public partial class EventManagerForm : Window {

        private bool EventsPaused = false;

        public void PauseEvents() {
            Debug.WriteLine("Events are paused on main");
            EventsPaused = true;
        }

        public void ResumeEvents() {
            Debug.WriteLine("Events are resumed on main");
            EventsPaused = false;
        }

        public event EventHandler<NamedEventArgs> OnNamedEvent = delegate { };

        public void InvokeNamedEvent(EventName eventName) {
            if (this.EventsPaused) return;
            Debug.WriteLine($"InvokeNamedEvent w/o data {eventName}");
            NamedEventArgs args = new(eventName);
            this.OnNamedEvent.Invoke(this, args);

            if (args.Handled == false) {
                Logger.Log($"Warning: Event '{eventName}' not handled.");
            }
        }

        public void InvokeNamedEvent(EventName eventName, ArgTable data) {
            if (this.EventsPaused) return;
            Debug.WriteLine($"InvokeNamedEvent w/ data {eventName}");
            NamedEventArgs args = new(eventName, data);
            this.OnNamedEvent.Invoke(this, args);

            if (args.Handled == false) {
                Logger.Log($"Warning: Event '{eventName}' not handled.");
            }
        }
    }
}
