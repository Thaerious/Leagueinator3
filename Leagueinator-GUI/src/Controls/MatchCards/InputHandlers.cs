
using System.Windows.Controls;
using System.Windows.Input;

namespace Leagueinator.GUI.Controls.MatchCards {
    public static class InputHandlers {
        public static void OnlyNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        /// <summary>
        /// Force and update and move to the next control when Enter is pressed on a TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void NextControlOnEnter(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (sender is TextBox textBox) {
                    // Move focus To the next control in the tab order
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    textBox.MoveFocus(request);
                }
            }
        }

        /// <summary>
        /// Replaces empty textboxes with "0" and selects the text.
        /// </summary>
        public static bool PreventEmpty(this TextBox textBox) {
            if (textBox.Text.Trim() == "") {
                textBox.Text = "0";
                textBox.SelectAll();
                return true;
            }
            return false;
        }
    }
}
