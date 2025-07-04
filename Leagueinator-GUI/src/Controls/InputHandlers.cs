
using System.Windows.Controls;
using System.Windows.Input;

namespace Leagueinator.GUI.Controls {
    public static class InputHandlers {
        public static void OnlyNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        /// <summary>
        /// Force and update and move to the next control when Enter is pressed on a TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void TxtNextOnEnter(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (sender is TextBox textBox) {
                    // Trigger binding update
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                    // Move focus To the next control in the tab order
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    textBox.MoveFocus(request);
                }
            }
        }
    }
}
