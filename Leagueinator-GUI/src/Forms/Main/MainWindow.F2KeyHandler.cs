using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Dialogs;
using System.Windows.Controls;
using System.Windows.Input;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow {

        /// <summary>
        /// Main window key-down handler To catch F2 for renaming players.
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
