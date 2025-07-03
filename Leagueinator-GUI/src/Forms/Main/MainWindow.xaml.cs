using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Leagueinator.GUI.Forms.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Button CurrentRoundButton { get; set; }

        public MainWindow() {
            this.InitializeComponent();
            this.Title = "Leagueinator []";
            //this.InstantiateDragDropHnd();
            this.CurrentRoundButton = this.AddRoundButton();
            this.CurrentRoundButton.Focus();
        }

        public void Ready() {
            this.InvokeFileEvent("New");
            this.InvokeRoundButton();
        }

        public void ClearFocus() {
            FocusManager.SetFocusedElement(this, null);
        }

        public MatchCard GetMatchCard(int lane) {
            var descendants = this.GetDescendantsOfType<MatchCard>();
            return this.GetDescendantsOfType<MatchCard>().First(MatchCard => MatchCard.Lane == lane);
        }

        public void PopulateMatchCards(RoundData roundData) {
            this.Dispatcher.InvokeAsync(new Action(() => {
                this.DoPopulateMatchCards(roundData);
            }), DispatcherPriority.Background);
        }

        /// <summary>
        /// Fill matchRow cards with values from "roundRow".
        /// Clears all matchRow cards that does not have a value in "roundRow".
        /// </summary>
        /// <param name="roundRow"></param>
        private void DoPopulateMatchCards(RoundData roundData) {
            int cardsToLoad = 0;
            int cardsLoaded = 0;

            // Clear all match cards that are not in the round data.
            while (roundData.Count < this.MatchCardStackPanel.Children.Count) {
                this.MatchCardStackPanel.Children.RemoveAt(0);
            }

            for (int i = 0; i < roundData.Count; i++) {
                MatchData matchData = roundData[i];

                if (i >= this.MatchCardStackPanel.Children.Count) {
                    MatchCard mc = MatchCardFactory.GenerateMatchCard(matchData.MatchFormat);
                    mc.Lane = matchData.Lane;
                    mc.Ends = matchData.Ends;
                    this.MatchCardStackPanel.Children.Add(mc);

                    mc.Loaded += (s, e) => {
                        mc.UpdateData(matchData);
                    };
                    continue;
                }

                MatchCard matchCard = (MatchCard)this.MatchCardStackPanel.Children[i];

                // If the matchCard format does not match the matchData format, replace it.
                if (matchCard.MatchFormat != matchData.MatchFormat) {
                    this.MatchCardStackPanel.Children.Remove(matchCard);
                    matchCard = MatchCardFactory.GenerateMatchCard(matchData.MatchFormat);
                    matchCard.Lane = matchData.Lane;
                    matchCard.Ends = matchData.Ends;
                    this.MatchCardStackPanel.Children.Insert(i, matchCard);
                    cardsToLoad++;

                    matchCard.Loaded += (s, e) => {
                        matchCard.UpdateData(matchData);
                        cardsLoaded++;

                        Debug.WriteLine($"MatchCard loaded: {cardsLoaded} / {cardsToLoad}");

                        if (cardsLoaded == cardsToLoad) {
                            this.AssignTabOrder();
                        }
                    };
                    continue;
                }

                // If the matchCard is already loaded, update it.
                matchCard.UpdateData(matchData);
            }
        }

        private void AssignTabOrder() {
            int nextTabIndex = 0;

            var elements = this.MatchCardStackPanel.FindByTag("PlayerName")
                .OfType<TextBox>();

            Debug.WriteLine($"Assigning TabIndex to {elements.Count()} elements.");

            foreach (var textBox in elements) {
                textBox.TabIndex = nextTabIndex++;
            }
        }

        public void RemoveRound(int index) {
            // Remove the button
            this.RoundButtonContainer.Children.RemoveAt(index);

            // Rename buttons
            int i = 1;
            foreach (Button button in this.RoundButtonContainer.Children) {
                button.Content = $"Round {i++}";
            }
        }

        public void RemoveMatch(int index) {
            this.MatchCardStackPanel.Children.RemoveAt(index);
        }
    }
}
