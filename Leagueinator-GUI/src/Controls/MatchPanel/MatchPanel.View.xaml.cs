using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls.MatchCards;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model.ViewModel;
using Leagueinator.Utility.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls.MatchPanel {
    /// <summary>
    /// View logic for MatchPanel.xaml
    /// </summary>
    public partial class MatchPanel : UserControl {
        private List<string> NameAlerts = [];

        public MatchPanel() {
            NamedEvent.RegisterHandler(this, true);
            InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            this.ResumeEvents();
        }

        /// <summary>
        /// Fill matchRow cards with values from "roundRow".
        /// Clears all matchRow cards that does not have a value in "roundRow".
        /// </summary>
        /// <param name="roundRow"></param>
        private void DoPopulateMatchCards(List<MatchRecord> matchRecords, List<PlayerRecord> playerRecords,
                                          List<string> nameAlerts, HashSet<int> laneAlerts) {
            int cardsToLoad = 0;
            int cardsLoaded = 0;

            this.OuterPanel.Children.Clear();

            foreach (MatchRecord matchRecord in matchRecords) {
                var matchCard = MatchCardFactory.GenerateMatchCard(matchRecord.MatchFormat);

                this.OuterPanel.Children.Add(matchCard);
                cardsToLoad++;

                // Assign tab order & set data when loaded
                matchCard.Loaded += (s, e) => {
                    matchCard.Lane = matchRecord.Lane;
                    matchCard.SetEnds(matchRecord.Ends);
                    matchCard.SetTieBreaker(matchRecord.TieBreaker);
                    matchCard.SetBowls(matchRecord.Score);
                    cardsLoaded++;

                    // When all of the match cards are loaded
                    if (cardsLoaded == cardsToLoad) {
                        this.AssignTabOrder();
                        this.SetPlayerNames(playerRecords);
                        this.HighlightNames(nameAlerts);
                        this.HighlightLanes(laneAlerts);
                    }
                };
            }
        }

        private void AssignTabOrder() {
            int nextTabIndex = 0;
            var elements = this.IsTagged("PlayerName").OfType<TextBox>();
            foreach (var textBox in elements) textBox.TabIndex = nextTabIndex++;
        }

        public MatchCard GetMatchCard(int lane) {
            var descendants = this.GetDescendantsOfType<MatchCard>();
            return this.GetDescendantsOfType<MatchCard>().First(MatchCard => MatchCard.Lane == lane);
        }

        private void SetPlayerNames(IEnumerable<PlayerRecord> playerRecords) {
            foreach (PlayerRecord roundRecord in playerRecords) {
                MatchCard matchCard = (MatchCard)this.OuterPanel.Children[roundRecord.Lane];
                TeamCard teamCard = matchCard.GetTeamCard(roundRecord.Team);
                teamCard[roundRecord.PlayerPos] = roundRecord.Name;
            }
        }

        public void RemoveMatch(int index) {
            this.OuterPanel.Children.RemoveAt(index);
        }

        private void HighlightLanes(HashSet<int> laneAlerts) {
            this.Descendants().OfType<MatchCard>()
                              .ToList()
                              .ForEach(mc => {
                                  InfoCard infoCard = mc.Descendants().OfType<InfoCard>().First();

                                  if (laneAlerts.Contains(infoCard.Lane)) {
                                      infoCard.LblLane.Foreground = AppColors.LaneAlert;
                                  }
                                  else {
                                      infoCard.LblLane.Foreground = SystemColors.ControlTextBrush;
                                  }
                              });
        }

        private void HighlightNames(List<string> nameAlerts) {
            this.NameAlerts = nameAlerts;

            this.Descendants().OfType<TextBox>()
                              .Where(textBox => textBox.HasTag("PlayerName"))
                              .ToList()
                              .ForEach(textBox => {
                                  if (nameAlerts.Contains(textBox.Text)) {
                                      textBox.Background = AppColors.TextBoxAlert;
                                  }
                                  else {
                                      textBox.Background = SystemColors.WindowBrush;
                                  }
                              });
        }
    }
}
