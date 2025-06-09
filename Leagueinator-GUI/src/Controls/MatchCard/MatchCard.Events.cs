using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls {
    public abstract partial class MatchCard : UserControl {

        public class MatchCardUpdateArgs(object source, string field, object oldValue, object newValue) 
                   : RoutedEventArgs(MatchCardUpdateEvent, source) {
            
            public string Field { get; } = field;
            public object OldValue { get; } = oldValue;
            public object NewValue { get; } = newValue;
            public int Lane { get; internal set; }

            public int? Team { get; internal set; } = null; // Assuming Team is optional, can be set later if needed
        }

        public static readonly RoutedEvent MatchCardUpdateEvent = EventManager.RegisterRoutedEvent(
            "MatchCardUpdate",           // Event name
            RoutingStrategy.Bubble,      // Bubble, Tunnel, or Direct
            typeof(RoutedEventHandler),  // Delegate type
            typeof(MatchCard)            // Owner type
        );

        public delegate void MatchCardUpdateHnd(object sender, MatchCardUpdateArgs e);

        public void InvokeEvent(string field, object oldValue, object newValue, int? teamIndex = null) {
            Debug.WriteLine($"Invoking event: {field}, Old: {oldValue}, New: {newValue}, Team: {teamIndex}");

            var args = new MatchCardUpdateArgs(this, field, oldValue, newValue) {
                Team = teamIndex,
                Lane = this.Lane - 1 // Lane is 1-based index for display, convert to 0-based index for internal use
            };
            this.RaiseEvent(args);
        }
    }
}
