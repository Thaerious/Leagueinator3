namespace Leagueinator.GUI.Controllers.NamedEvents {
    /// <summary>
    /// Attribute used to mark methods as handlers for a specific named event.
    /// </summary>
    /// <remarks>
    /// This allows you to decorate methods with metadata that ties them
    /// to an <see cref="EventName"/> identifier. Any events emitted with the 
    /// specified event name will result in the annotated method getting invoked.
    /// 
    /// A single method can respond to multiple values by applying multiple 
    /// attributes.
    /// 
    /// Event handling can be paused by using the PauseEvents extension method, and
    /// resumed using the ResumeEvents extension method.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]    
    public sealed class NamedEventHandler : Attribute {

        /// <summary>
        /// The event name that the handler responds to.
        /// </summary>
        public EventName EventName { get; }

        /// <summary>
        /// Construct a new handler attribute for the given event.
        /// </summary>
        public NamedEventHandler(EventName eventName) {
            this.EventName = eventName;
        }
    }
}
