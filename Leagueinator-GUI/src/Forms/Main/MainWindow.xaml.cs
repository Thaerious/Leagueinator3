using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
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
            var descendants = this.Descendants<MatchCard>();
            return this.Descendants<MatchCard>().First(MatchCard => MatchCard.Lane == lane);
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
                        this.UpdateMatchCard(mc, matchData);
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

                    matchCard.Loaded += (s, e) => {
                        this.UpdateMatchCard(matchCard, matchData);
                    };
                    continue;
                }

                // If the matchCard is already loaded, update it.
                this.UpdateMatchCard(matchCard, matchData);
            }
        }

        private void UpdateMatchCard(MatchCard matchCard, MatchData matchData) {
            matchCard.Lane = matchData.Lane;
            matchCard.Ends = matchData.Ends;
            matchCard.SetTieBreaker(matchData.TieBreaker);

            matchCard.SuppressBowlsEvent = true;

            for (int team = 0; team < matchData.Teams.Length; team++) {
                for (int position = 0; position < matchData.Teams[team].Length; position++) {
                    TeamCard teamCard = matchCard.GetTeamCard(team)!;
                    var name = matchData.Teams[team][position];
                    teamCard.SetName(name, position);
                    teamCard.Bowls = matchData.Score[team];
                }
            }

            matchCard.SuppressBowlsEvent = false;
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
