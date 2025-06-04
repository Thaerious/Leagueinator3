using System.Windows;

namespace Leagueinator.GUI.Controls {
    public class DragBeginArgs(RoutedEvent routedEvent, FrameworkElement from) : RoutedEventArgs(routedEvent) {
        public FrameworkElement From = from;
    }

    public class DragEndArgs(RoutedEvent routedEvent, FrameworkElement from, FrameworkElement to) : RoutedEventArgs(routedEvent) {
        public FrameworkElement From = from;
        public FrameworkElement To = to;
    }

    public class DragDropDelegates {
        public delegate void DragBegin(object sender, DragBeginArgs args);
        public delegate void DragEnd(object sender, DragEndArgs args);
    }
}
