using Leagueinator.GUI.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Leagueinator.GUI.Forms.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private EventData EventData { get; set; } = new();

        public MainWindow() {
            this.InitializeComponent();
            this.Title = "Leagueinator []";
            //this.InstantiateDragDropHnd();
            DataButton<RoundData> button = this.AddRoundButton();
            button.Focus();
            this.PopulateMatchCards(button.Data!);

            this.AddHandler(MatchCard.MyCustomEvent, new RoutedEventHandler(OnPlayerNameChanged));
        }

        private void OnPlayerNameChanged(object sender, RoutedEventArgs e) {
            if (e is MatchCardNameChangedArgs args) {
                Debug.WriteLine($"In MainWindow Lane {args.LaneIndex}, Team {args.TeamIndex}: {args.OldName} → {args.NewName}");
            }
        }

        public void ClearFocus() {
            FocusManager.SetFocusedElement(this, null);
        }

        public void NewEvent() {

        }

        /// <summary>
        /// Fill matchRow cards with values from "roundRow".
        /// Clears all matchRow cards that does not have a value in "roundRow".
        /// </summary>
        /// <param name="roundRow"></param>
        private void PopulateMatchCards(RoundData roundData) {
            Debug.WriteLine($"PopulateMatchCards: {roundData.Count} matches.");

            // Remove all match cards from panel.
            while (this.MatchCardStackPanel.Children.Count > 0) {
                var child = this.MatchCardStackPanel.Children[0];
                this.MatchCardStackPanel.Children.Remove(child);
            }

            // Add match cards for each match in roundRow.
            foreach (MatchData matchData in roundData) {                
                MatchCard matchCard = MatchCardFactory.GenerateMatchCard(matchData.MatchFormat);
                matchCard.Lane = this.MatchCardStackPanel.Children.Count + 1;
                this.MatchCardStackPanel.Children.Add(matchCard);
            }
        }
    }
}
