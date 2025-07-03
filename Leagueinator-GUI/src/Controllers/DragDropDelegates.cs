using Leagueinator.GUI.src.Controllers;
using System.Windows;

namespace Leagueinator.GUI.Controllers {
    public class DragBeginArgs(RoutedEvent routedEvent, FrameworkElement from) 
               : RoutedEventArgs(routedEvent) {

        public FrameworkElement From = from;
    }

    public class DragEndArgs(RoutedEvent routedEvent, FrameworkElement from, FrameworkElement to) 
               : RoutedEventArgs(routedEvent) {

        public FrameworkElement From = from;
        public FrameworkElement To = to;
    }

    public class ClearFocusArgs(RoutedEvent routedEvent)
               : RoutedEventArgs(routedEvent) {
    }

    public class RequestFocusArgs(RoutedEvent routedEvent, TeamID target, bool append) 
               : RoutedEventArgs(routedEvent) {
        public TeamID Target = target;
        public bool Append = append;    
    }

    public class DragDropDelegates {
        public delegate void DragBegin(object sender, DragBeginArgs args);
        public delegate void DragEnd(object sender, DragEndArgs args);
        public delegate void RequestFocus(object sender, RequestFocusArgs args);
    }
}
