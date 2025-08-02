using Leagueinator.GUI.Controllers;
using System.Windows;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {

        public event RoutedEventHandler OnDragEnd {
            add => AddHandler(DragDropController.RegisteredDragEndEvent, value);
            remove => RemoveHandler(DragDropController.RegisteredDragEndEvent, value);
        }
        private void HndClearFocus(object? sender, EventArgs? _) {
            ClearFocusArgs args = new(DragDropController.RequestFocusEvent);
            this.RaiseEvent(args);
        }
    }
}
