using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls {
    public abstract partial class MatchCard : UserControl {

        public class MatchCardEventArgs(object source, string field) 
                   : RoutedEventArgs(MatchCardUpdateEvent, source) {
            public string Field { get; } = field;
            public int Lane { get; internal set; }

            public int? Team { get; internal set; } = null; 
            public int? Position { get; internal set; } = null; 
        }

        public class MatchCardNewArgs(object source, string field, object newValue)
                   : MatchCardEventArgs(source, field) {
            public object NewValue { get; } = newValue;
        }

        public class MatchCardUpdateArgs(object source, string field, object oldValue, object newValue) 
                   : MatchCardNewArgs(source, field, newValue) {
            public object OldValue { get; } = oldValue;
        }

        public static readonly RoutedEvent MatchCardUpdateEvent = EventManager.RegisterRoutedEvent(
            "MatchCardUpdate",           // Event name
            RoutingStrategy.Bubble,      // Bubble, Tunnel, or Direct
            typeof(RoutedEventHandler),  // Delegate type
            typeof(MatchCard)            // Owner type
        );

        public delegate void MatchCardUpdateHnd(object sender, MatchCardUpdateArgs e);

        public void InvokeEvent(string field, int? teamIndex = default, int? position = default) {
            var args = new MatchCardEventArgs(this, field) {
                Team = teamIndex,
                Position = position,
                Lane = this.Lane - 1 // Lane is 1-based index for display, convert to 0-based index for internal use
            };
            this.RaiseEvent(args);
        }

        public void InvokeEvent(string field, object newValue, int? teamIndex = default, int? position = default) {
            var args = new MatchCardNewArgs(this, field, newValue) {
                Team = teamIndex,
                Position = position,
                Lane = this.Lane - 1 // Lane is 1-based index for display, convert to 0-based index for internal use
            };
            this.RaiseEvent(args);
        }

        public void InvokeEvent(string field, object oldValue, object newValue, int? teamIndex = null, int? position = default) {
            var args = new MatchCardUpdateArgs(this, field, oldValue, newValue) {
                Team = teamIndex,
                Position = position,
                Lane = this.Lane - 1 // Lane is 1-based index for display, convert to 0-based index for internal use
            };
            this.RaiseEvent(args);
        }
    }
}
