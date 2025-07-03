using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Controllers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow {
        public event RoutedEventHandler OnMatchCardUpdate {
            add => AddHandler(MatchCard.MatchCardUpdateEvent, value);
            remove => RemoveHandler(MatchCard.MatchCardUpdateEvent, value);
        }

        public event RoutedEventHandler OnDragEnd {
            add => AddHandler(DragDropController.RegisteredDragEndEvent, value);
            remove => RemoveHandler(DragDropController.RegisteredDragEndEvent, value);
        }

        public event RoutedEventHandler OnRequestFocus {
            add => AddHandler(DragDropController.RequestFocusEvent, value);
            remove => RemoveHandler(DragDropController.RequestFocusEvent, value);
        }

        internal void HighLightRound(int index) {
            foreach (Button button in this.RoundButtonContainer.Children) {
                button.Background = Brushes.LightGray;
            }

            Button selected = (Button)this.RoundButtonContainer.Children[index];
            selected.Background = Brushes.Green;
        }

        /// <summary>
        /// MainWindow window key-down handler To catch F2 for renaming players.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HndKeyDownRenamePlayer(object sender, KeyEventArgs e) {
            if (e.Key != Key.F2) return;
            if (Keyboard.FocusedElement is not TextBox textBox) return;

            this.ClearFocus();
            string oldName = textBox.Text;
            RenameDialog dialog = new RenameDialog(oldName);

            if (dialog.ShowDialog() == true) {
                // TODO: Implement rename event.                
            }
        }
    }
}
