using Leagueinator.GUI.Controls;
using System.Windows;
using System.Windows.Media;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {
        private DataButton<RoundData>? CurrentRoundButton;

        /// <summary>
        /// Event args for round data creation.
        /// </summary>
        public class RoundDataEventArgs : EventArgs {
            public RoundData? RoundData { get; }
            public RoundDataEventArgs(RoundData? roundData) {
                this.RoundData = roundData;
            }
        }

        /// <summary>
        /// Event triggered when new round data is created.
        /// </summary>
        public event EventHandler<RoundDataEventArgs>? OnRoundDataCreated;

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
        private DataButton<RoundData> AddRoundButton(RoundData? source = null) {
            source = source ?? new RoundData(this.EventData.MatchFormat, this.EventData.LaneCount, this.EventData.DefaultEnds);

            DataButton<RoundData> button = new() {
                Content = $"Round {this.RoundButtonContainer.Children.Count + 1}",
                Margin = new Thickness(3),
                Data = source
            };

            this.RoundButtonContainer.Children.Add(button);
            button.Click += this.HndClickSelectRound;

            // Trigger custom event for round data creation
            this.OnRoundDataCreated?.Invoke(this, new RoundDataEventArgs(button.Data));

            return button;
        }

        /// <summary>
        /// Triggered when a round button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="_"></param>
        /// <exception cref="NotSupportedException"></exception>
        private void HndClickSelectRound(object? sender, EventArgs? _) {
            if (sender is not DataButton<RoundData> button) throw new NotSupportedException();
            this.ClearFocus();

            if (this.CurrentRoundButton is not null) {
                this.CurrentRoundButton.Background = Brushes.LightGray;
            }

            this.CurrentRoundButton = button;
            button.Background = Brushes.LightCyan;

            this.PopulateMatchCards();
        }

        private void InvokeRoundButton(DataButton<RoundData>? button = null) {
            button ??= (DataButton<RoundData>)this.RoundButtonContainer.Children[^1];
            this.CurrentRoundButton = button;
            this.PopulateMatchCards();
        }
    }
}
