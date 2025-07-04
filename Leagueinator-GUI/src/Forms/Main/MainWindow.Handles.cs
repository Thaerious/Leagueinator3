using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.src.Controllers;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {

        /// <summary>
        /// Set a new select-round button.
        /// </summary>
        /// <param name="roundRow">The associated roundRow</param>
        /// <returns></returns>
        public Button AddRoundButton() {
            Button button = new() {
                Content = $"Round {this.RoundButtonContainer.Children.Count + 1}",
                Margin = new Thickness(3)
            };

            this.RoundButtonContainer.Children.Add(button);
            button.Click += this.HndClickSelectRound;
            return button;
        }

        private void InvokeRoundButton(Button? button = null) {
            button ??= (Button)this.RoundButtonContainer.Children[^1];
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
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
            var index = this.RoundButtonContainer.Children.IndexOf(button);
            this.InvokeNamedEvent(EventName.SelectRound, index);
        }

        private void HndClearFocus(object? sender, EventArgs? _) {
            ClearFocusArgs args = new(DragDropController.RequestFocusEvent);
            this.RaiseEvent(args);
        }
    }
}
