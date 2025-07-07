using Leagueinator.GUI.Utility;

namespace Leagueinator.GUI.Controllers.NamedEvents {
    /// <summary>
    /// Manages named events that can be paused and resumed.
    /// Allows subscribers to handle events by name and optional data payload.
    /// </summary>
    public class NamedEventDispatcher(object owner) {

        // Flag to control whether events should be dispatched or ignored
        private bool EventsPaused = false;

        // Parent object that owns this dispatcher.
        private readonly object Owner = owner;

        /// <summary>
        /// Prevents any events from being invoked until resumed.
        /// </summary>
        public void PauseEvents() {
            EventsPaused = true;
        }

        /// <summary>
        /// Resumes event invocation after being paused.
        /// </summary>
        public void ResumeEvents() {
            EventsPaused = false;
        }

        /// <summary>
        /// Event triggered when a named event is invoked.
        /// Subscribers can handle different event names and optionally modify the event args.
        /// </summary>
        public event EventHandler<NamedEventArgs> OnNamedEvent = delegate { };

        public static NamedEventDispatcher operator +(NamedEventDispatcher disp, NamedEventReceiver rcv) {
            disp.OnNamedEvent += rcv.NamedEventHnd;
            return disp;
        }

        public static NamedEventDispatcher operator -(NamedEventDispatcher disp, NamedEventReceiver rcv) {
            disp.OnNamedEvent -= rcv.NamedEventHnd;
            return disp;
        }

        /// <summary>
        /// Invokes a named event with no data payload.
        /// Logs a warning if no handler marks it as handled.
        /// </summary>
        public void Dispatch(EventName eventName) {
            if (this.EventsPaused) return;
            Logger.Log($"Event '{eventName}' dispatched by '{this.Owner.GetType().Name}'.");

            NamedEventArgs args = new(eventName);
            this.OnNamedEvent.Invoke(this.Owner, args);

            if (args.Handled == false) {
                Logger.Log($"Warning: Event not handled '{eventName}' from '{this.Owner.GetType().Name}'.");
            }
        }

        /// <summary>
        /// Invokes a named event with a data payload (ArgTable).
        /// Logs a warning if no handler marks it as handled.
        /// </summary>
        public void Dispatch(EventName eventName, ArgTable data) {
            if (this.EventsPaused) return;
            Logger.Log($"Event '{eventName}' dispatched by '{this.Owner.GetType().Name}'.");

            NamedEventArgs args = new(eventName, data);
            this.OnNamedEvent.Invoke(this.Owner, args);

            if (args.Handled == false) {
                Logger.Log($"Warning: Event not handled '{eventName}' from '{this.Owner.GetType().Name}'.");
            }
        }
    }
}
