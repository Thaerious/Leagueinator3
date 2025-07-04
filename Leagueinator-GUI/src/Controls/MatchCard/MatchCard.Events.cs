using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls {
    public abstract partial class MatchCard : UserControl {

        public class MatchCardEventArgs(object source, string field, bool echo = false) 
                   : RoutedEventArgs(MatchCardUpdateEvent, source) {
            public string Field { get; } = field;
            public int Lane { get; internal set; }

            public bool Echo { get; } = echo;   

            public int? Team { get; internal set; } = null; 
            public int? Position { get; internal set; } = null; 
        }

        public class MatchCardNewArgs(object source, string field, object newValue, bool echo = false)
                   : MatchCardEventArgs(source, field, echo) {
            public object NewValue { get; } = newValue;
        }

        public class MatchCardUpdateArgs(object source, string field, object oldValue, object newValue, bool echo = false) 
                   : MatchCardNewArgs(source, field, newValue, echo) {
            public object OldValue { get; } = oldValue;
        }

        public static readonly RoutedEvent MatchCardUpdateEvent = EventManager.RegisterRoutedEvent(
            "MatchCardUpdate",           // Event name
            RoutingStrategy.Bubble,      // Bubble, Tunnel, or Direct
            typeof(RoutedEventHandler),  // Delegate type
            typeof(MatchCard)            // Owner type
        );

        public delegate void MatchCardUpdateHnd(object sender, MatchCardUpdateArgs e);
    }
}
