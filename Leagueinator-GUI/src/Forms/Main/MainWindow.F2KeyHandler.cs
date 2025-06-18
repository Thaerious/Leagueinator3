using Leagueinator.GUI.Dialogs;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow {
        internal void HighLightRound(int index) {
            Debug.WriteLine($"MainWindow.HighLightRound: {index}");
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
