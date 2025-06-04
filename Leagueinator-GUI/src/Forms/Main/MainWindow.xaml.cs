using System.Windows;
using System.Windows.Input;

namespace Leagueinator.GUI.Forms.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            this.InitializeComponent();
            this.Title = "Leagueinator []";
            //this.InstantiateDragDropHnd();
        }

        public void ClearFocus() {
            FocusManager.SetFocusedElement(this, null);
        }

        /// <summary>
        /// Fill matchRow cards with values from "roundRow".
        /// Clears all matchRow cards that does not have a value in "roundRow".
        /// </summary>
        /// <param name="roundRow"></param>
        //private void PopulateMatchCards(RoundRow roundRow) {
        //    if (this.EventRow is null) throw new NullReferenceException();

        //    // Remove all match cards from panel.
        //    while (this.MatchCardStackPanel.Children.Count > 0) {
        //        var child = this.MatchCardStackPanel.Children[0];
        //        this.MatchCardStackPanel.Children.Remove(child);
        //    }

        //    List<MatchRow> matchRows = [.. roundRow.Matches];
        //    matchRows.Sort((m1, m2) => m1.Lane.CompareTo(m2.Lane));

        //    foreach (MatchRow matchRow in matchRows) {
        //        MatchCard matchCard = MatchCardFactory.GenerateMatchCard(matchRow);
        //        this.MatchCardStackPanel.Children.Add(matchCard);
        //    }
        //}
    }
}
