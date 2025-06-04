using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MatchCardV1 = Leagueinator.GUI.Controls.MatchCardV1;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {
        private Button? CurrentRoundButton;

        /// <summary>
        /// Triggered when a match card changes type.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void HndMatchFormatChanged(object source, RoutedEventArgs args) {
            // TODO: Implement match format change handling.
        }

        /// <summary>
        /// Set a new select-round button.
        /// </summary>
        /// <param name="roundRow">The associated roundRow</param>
        /// <returns></returns>
        private Button AddRoundButton() {
            Button button = new() {
                Content = $"Round {this.RoundButtonContainer.Children.Count + 1}",
                Margin = new Thickness(3),
            };

            this.RoundButtonContainer.Children.Add(button);
            button.Click += this.HndClickSelectRound;
            return button;
        }

        /// <summary>
        /// Triggered when a round button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="_"></param>
        /// <exception cref="NotSupportedException"></exception>
        private void HndClickSelectRound(object? sender, EventArgs? _) {
            if (sender is not Button button) throw new NotSupportedException();
            this.ClearFocus();

            if (this.CurrentRoundButton is not null) {
                this.CurrentRoundButton.Background = Brushes.LightGray;
            }

            this.CurrentRoundButton = button;
            button.Background = Brushes.LightCyan;

            // TODO: Implement round selection event.
        }

        private void InvokeRoundButton(Button? button = null) {
            button ??= (Button)this.RoundButtonContainer.Children[^1];
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}
