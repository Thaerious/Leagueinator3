using Leagueinator.GUI.Utility;

namespace Leagueinator.GUI.Controllers.NamedEvents {
    /// <summary>
    /// Manages named events that can be paused and resumed.
    /// Allows subscribers to handle events by name and optional data payload.
    /// </summary>
    public class NamedEventDispatcher() {

        // Flag to control whether events should be dispatched or ignored
        private bool EventsPaused = false;

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
        /// Invokes a named event with no data payload.
        /// Logs a warning if no handler marks it as handled.
        /// </summary>
        public void Dispatch(EventName eventName) {
            if (this.EventsPaused) return;
            Logger.Log($"Event '{eventName}' dispatched.");

            NamedEventArgs args = new(eventName);
            NamedEvent.InvokeHandlers(args);

            if (args.Handled == false) {
                Logger.Log($"Warning: Event not handled '{eventName}'.");
            }
        }

        /// <summary>
        /// Invokes a named event with a data payload (ArgTable).
        /// Logs a warning if no handler marks it as handled.
        /// </summary>
        public void Dispatch(EventName eventName, ArgTable data) {
            if (this.EventsPaused) return;
            Logger.Log($"Event '{eventName}' dispatched by.");

            NamedEventArgs args = new(eventName, data);
            NamedEvent.InvokeHandlers(args);

            if (args.Handled == false) {
                Logger.Log($"Warning: Event not handled '{eventName}'.");
            }
        }
    }
}
