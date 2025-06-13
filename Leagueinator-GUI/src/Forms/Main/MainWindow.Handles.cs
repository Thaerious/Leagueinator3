using Leagueinator.GUI.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {
        private Button CurrentRoundButton;

        /// <summary>
        /// Event args for round data creation.
        /// </summary>
        public class RoundDataEventArgs(string action, int index) : EventArgs {
            public int Index { get; private set; } = index;
            public string Action { get; private set; } = action;
        }

        /// <summary>
        /// Event triggered when new round data is created.
        /// </summary>
        public event EventHandler<RoundDataEventArgs>? OnRoundData;

        /// <summary>
        /// Set a new select-round button.
        /// </summary>
        /// <param name="roundRow">The associated roundRow</param>
        /// <returns></returns>
        private Button AddRoundButton() {
            Button button = new() {
                Content = $"Round {this.RoundButtonContainer.Children.Count + 1}",
                Margin = new Thickness(3)
            };

            this.RoundButtonContainer.Children.Add(button);
            button.Click += this.HndClickSelectRound;
            var index = this.RoundButtonContainer.Children.Count - 1;

            this.InvokeRoundEvent(action: "Create", index: index);

            return button;
        }

        private void InvokeRoundButton(Button? button = null) {
            button ??= (Button)this.RoundButtonContainer.Children[^1];
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void InvokeRoundEvent(string action, int index) {
            this.Dispatcher.BeginInvoke(new Action(() => {
                var args = new RoundDataEventArgs(action, index);
                this.OnRoundData?.Invoke(this, args);
            }));
        }

        private void InvokeRoundEvent(string action) {
            var args = new RoundDataEventArgs(action, -1);
            this.OnRoundData?.Invoke(this, args);
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
            button.Background = Brushes.Green;

            var index = this.RoundButtonContainer.Children.IndexOf(button);
            this.InvokeRoundEvent(action: "Select", index: index);
        }
    }
}
