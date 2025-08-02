using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Leagueinator.Utility.Extensions;
using Leagueinator.GUI.Controls.MatchCards;

namespace Leagueinator.GUI.Forms.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            this.InitializeComponent();
            this.Title = "Leagueinator []";
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            this.RoundPanel.PreviewMouseDown += (s, e) => {
                Keyboard.ClearFocus();
            };
            this.MatchCardStackPanel.PreviewMouseDown += (s, e) => {
                Keyboard.ClearFocus();
            };
        }

        public void ClearFocus() {
            FocusManager.SetFocusedElement(this, null);
        }

        public MatchCard GetMatchCard(int lane) {
            var descendants = this.GetDescendantsOfType<MatchCard>();
            return this.GetDescendantsOfType<MatchCard>().First(MatchCard => MatchCard.Lane == lane);
        }

        public void PopulateMatchCards(RoundRecordList roundRecords) {
            this.Dispatcher.InvokeAsync(new Action(() => {
                this.DoPopulateMatchCards(roundRecords);
            }), DispatcherPriority.Background);
        }

        /// <summary>
        /// Fill matchRow cards with values from "roundRow".
        /// Clears all matchRow cards that does not have a value in "roundRow".
        /// </summary>
        /// <param name="roundRow"></param>
        private void DoPopulateMatchCards(RoundRecordList roundRecords) {
            int cardsToLoad = 0;
            int cardsLoaded = 0;

            this.MatchCardStackPanel.Children.Clear();

            foreach (MatchRecord matchRecord in roundRecords.Matches) {
                var matchCard = MatchCardFactory.GenerateMatchCard(matchRecord.MatchFormat);

                this.MatchCardStackPanel.Children.Add(matchCard);
                cardsToLoad++;

                // Assign tab order & set data when loaded
                matchCard.Loaded += (s, e) => {
                    this.PauseEvents();
                    matchCard.Lane = matchRecord.Lane;
                    matchCard.SetEnds(matchRecord.Ends);
                    matchCard.SetTieBreaker(matchRecord.TieBreaker);
                    matchCard.SetBowls(matchRecord.Score);
                    cardsLoaded++;

                    if (cardsLoaded == cardsToLoad) {
                        this.AssignTabOrder();
                        this.SetPlayerNames(roundRecords);
                        this.ResumeEvents();
                    }
                };
            }
        }

        private void SetPlayerNames(RoundRecordList roundRecords) {
            for (int i = 0; i < roundRecords.Players.Count; i++) {
                RoundRecord roundRecord = roundRecords.Players[i];
                MatchCard matchCard = (MatchCard)this.MatchCardStackPanel.Children[roundRecord.Lane];
                TeamCard teamCard = matchCard.GetTeamCard(roundRecord.Team);
                teamCard[roundRecord.Pos] = roundRecord.Name;
            }
        }

        private void AssignTabOrder() {
            int nextTabIndex = 0;
            var elements = this.MatchCardStackPanel.FindByTag("PlayerName").OfType<TextBox>();
            foreach (var textBox in elements) textBox.TabIndex = nextTabIndex++;
        }

        public void RemoveMatch(int index) {
            this.MatchCardStackPanel.Children.RemoveAt(index);
        }
    }
}
