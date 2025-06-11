using System.Diagnostics;
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

        public void InvokeEvent(string field, int? team = default, int? pos = default) {
            var args = new MatchCardEventArgs(this, field) {
                Team = team,
                Position = pos,
                Lane = this.Lane
            };
            this.RaiseEvent(args);
        }

        public void InvokeEvent(string field, object value, int? team = default, int? pos = default) {
            var args = new MatchCardNewArgs(this, field, value) {
                Team = team,
                Position = pos,
                Lane = this.Lane
            };
            this.RaiseEvent(args);
        }

        public void InvokeEvent(string field, object old, object value, int? team = null, int? pos = default) {
            var args = new MatchCardUpdateArgs(this, field, old, value) {
                Team = team,
                Position = pos,
                Lane = this.Lane
            };
            this.RaiseEvent(args);
        }
    }
}
