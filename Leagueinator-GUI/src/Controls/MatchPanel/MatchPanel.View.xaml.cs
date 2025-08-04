using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls.MatchCards;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.ViewModel;
using Leagueinator.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Leagueinator.GUI.Controls.MatchPanel {
    /// <summary>
    /// View logic for MatchPanel.xaml
    /// </summary>
    public partial class MatchPanel : UserControl {


        public MatchPanel() {
            NamedEvent.RegisterHandler(this, true);
            InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            this.ResumeEvents();
        }

        public void HndClearFocus(object sender, EventArgs e) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fill matchRow cards with values from "roundRow".
        /// Clears all matchRow cards that does not have a value in "roundRow".
        /// </summary>
        /// <param name="roundRow"></param>
        private void DoPopulateMatchCards(List<MatchRecord> matchRecords, List<PlayerRecord> playerRecords) {
            int cardsToLoad = 0;
            int cardsLoaded = 0;

            this.OuterPanel.Children.Clear();

            foreach (MatchRecord matchRecord in matchRecords) {
                var matchCard = MatchCardFactory.GenerateMatchCard(matchRecord.MatchFormat);

                this.OuterPanel.Children.Add(matchCard);
                cardsToLoad++;

                // Assign tab order & set data when loaded
                matchCard.Loaded += (s, e) => {
                    this.PauseEvents();
                    matchCard.Lane = matchRecord.Lane;
                    matchCard.SetEnds(matchRecord.Ends);
                    matchCard.SetTieBreaker(matchRecord.TieBreaker);
                    matchCard.SetBowls(matchRecord.Score);
                    cardsLoaded++;

                    // When all of the match cards are loaded
                    if (cardsLoaded == cardsToLoad) {
                        this.AssignTabOrder();
                        this.SetPlayerNames(playerRecords);
                        this.ResumeEvents();
                    }
                };
            }
        }

        private void AssignTabOrder() {
            int nextTabIndex = 0;
            var elements = this.FindByTag("PlayerName").OfType<TextBox>();
            foreach (var textBox in elements) textBox.TabIndex = nextTabIndex++;
        }

        public MatchCard GetMatchCard(int lane) {
            var descendants = this.GetDescendantsOfType<MatchCard>();
            return this.GetDescendantsOfType<MatchCard>().First(MatchCard => MatchCard.Lane == lane);
        }

        public void PopulateMatchCards(List<MatchRecord> matchRecords, List<PlayerRecord> playerRecords) {
            this.Dispatcher.InvokeAsync(new Action(() => {
                this.DoPopulateMatchCards(matchRecords, playerRecords);
            }), DispatcherPriority.Background);
        }

        private void SetPlayerNames(IEnumerable<PlayerRecord> playerRecords) {
            foreach (PlayerRecord roundRecord in playerRecords) {
                MatchCard matchCard = (MatchCard)this.OuterPanel.Children[roundRecord.Lane];
                TeamCard teamCard = matchCard.GetTeamCard(roundRecord.Team);
                teamCard[roundRecord.Pos] = roundRecord.Name;
            }
        }


        public void RemoveMatch(int index) {
            this.OuterPanel.Children.RemoveAt(index);
        }
    }
}
