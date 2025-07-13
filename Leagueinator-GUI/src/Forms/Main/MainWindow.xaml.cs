using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leagueinator.GUI.Forms.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public static NamedEventDispatcher NamedEventDisp { get; } = new();

        public MainWindow() {
            this.InitializeComponent();
            this.Title = "Leagueinator []";
            var button = this.AddRoundButton();
            button.Focus();

            this.Loaded += this.OnLoadDo;
        }

        private void OnLoadDo(object sender, RoutedEventArgs e) {
            this.RoundButtonStackPanel.PreviewMouseDown += (s, e) => {
                Keyboard.ClearFocus();
            };
            this.MatchCardStackPanel.PreviewMouseDown += (s, e) => {
                Keyboard.ClearFocus();
            };
        }

        public void Ready() {
            NamedEventDisp.Dispatch(EventName.NewLeague);
            this.InvokeRoundButton();
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
                    NamedEventDisp.PauseEvents();
                    matchCard.Lane = matchRecord.Lane;
                    matchCard.SetEnds(matchRecord.Ends);
                    matchCard.SetTieBreaker(matchRecord.TieBreaker);
                    matchCard.SetBowls(matchRecord.Score);
                    cardsLoaded++;

                    if (cardsLoaded == cardsToLoad) {
                        this.AssignTabOrder();
                        this.SetPlayerNames(roundRecords);
                        NamedEventDisp.ResumeEvents();
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

        public void RemoveRound(int index) {
            // Remove the button
            this.RoundButtonStackPanel.Children.RemoveAt(index);

            // Rename buttons
            int i = 1;
            foreach (Button button in this.RoundButtonStackPanel.Children) {
                button.Content = $"Round {i++}";
            }
        }

        public void RemoveMatch(int index) {
            this.MatchCardStackPanel.Children.RemoveAt(index);
        }

        internal void HighLightRound(int index) {
            foreach (Button button in this.RoundButtonStackPanel.Children) {
                button.Background = Brushes.LightGray;
            }

            Button selected = (Button)this.RoundButtonStackPanel.Children[index];
            selected.Background = Brushes.Green;
        }
    }
}
