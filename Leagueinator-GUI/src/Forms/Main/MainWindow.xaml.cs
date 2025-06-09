using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using static Leagueinator.GUI.Controls.MatchCard;

namespace Leagueinator.GUI.Forms.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private EventData _eventData = new();
        private EventData EventData {
            get => this._eventData;
            set {
                this._eventData = value;
                this.UpdateAllRoundData();
                this.PopulateMatchCards();
            }
        }

        private RoundData RoundData {
            get {
                if (this.CurrentRoundButton == null) {
                    throw new InvalidOperationException("CurrentRoundButton is not set. Cannot access round data.");
                }
                return this.CurrentRoundButton.Data!;
            }
        }

        public MainWindow() {
            this.InitializeComponent();
            this.Title = "Leagueinator []";
            //this.InstantiateDragDropHnd();
            DataButton<RoundData> button = this.AddRoundButton();
            this.CurrentRoundButton = button;
            button.Focus();
            this.PopulateMatchCards();

            this.AddHandler(MatchCard.MatchCardUpdateEvent, new RoutedEventHandler(OnMatchCardUpdate));
        }

        private void OnMatchCardUpdate(object sender, RoutedEventArgs e) {
            if (e is not MatchCardUpdateArgs args) return;
            if (args.Source is not MatchCard matchCard) return;

            if (args.Lane < 0 || args.Lane >= this.RoundData.Count) {
                throw new ArgumentOutOfRangeException(nameof(args.Lane), "Lane index is out of range.");
            }

            switch (args.Field) {
                case "Name":
                    if (args.Team is null) throw new ArgumentException("Team index must be provided for player name changes.");
                    this.OnPlayerNameChanged(args.Lane, (int)args.Team, (string)args.OldValue, (string)args.NewValue);
                    break;
                case "Ends":
                    this.RoundData[args.Lane].Ends = (int)args.NewValue;
                    break;
                case "Tie":
                    this.RoundData[args.Lane].TieBreaker = (int)args.NewValue;
                    break;
                case "Bowls":
                    if (args.Team is null) throw new ArgumentException("Team index must be provided for bowls changes.");
                    this.RoundData[args.Lane].Score[(int)args.Team] = (int)args.NewValue;
                    break;
            }
        }

        private void OnPlayerNameChanged(int lane, int team, string oldName, string newName) {
            var index = lane;

            if (oldName == "") {
                // Add new player with newName to the match data.
                PlayerData player = new(newName, team);
                this.RoundData[index].Players.Add(player);
            }
            else if (newName == "") {
                // Remove player with oldName from the match data.
                this.RoundData[index].Players.RemoveAll(p => p.Name == oldName);
            }
            else {
                // Replace oldName with newName in the match data.
                this.RoundData[index].Replace(oldName, newName);
            }
        }

        public void ClearFocus() {
            FocusManager.SetFocusedElement(this, null);
        }

        /// <summary>
        /// Iterates through all round buttons in the UI and synchronizes their associated <see cref="RoundData"/> 
        /// with the current event settings by calling <see cref="SyncRoundData"/>.
        /// <para>
        /// This ensures that each round's data structure matches the current event's lane count, match format, and default ends.
        /// </para>
        /// </summary>
        private void UpdateAllRoundData() {
            for (int i = 0; i < this.RoundButtonContainer.Children.Count; i++) {
                if (this.RoundButtonContainer.Children[i] is DataButton<RoundData> button) {
                    this.SyncRoundData(button.Data!);
                }
            }
        }

        /// <summary>
        /// Synchronizes the provided <paramref name="roundData"/> with the current event settings.
        /// <para>
        /// - Removes empty lanes if there are more lanes than specified in <see cref="EventData.LaneCount"/>.
        /// - Adds new empty lanes if there are fewer lanes than <see cref="EventData.LaneCount"/>.
        /// - Ensures all <see cref="MatchData.Lane"/> values are set to their correct indices.
        /// - Sets the ends for unplayed matches to <see cref="EventData.DefaultEnds"/>.
        /// - Updates the match format for empty matches to match <see cref="EventData.MatchFormat"/>.
        /// </para>
        /// </summary>
        /// <param name="roundData">The round data to update, representing a collection of matches for a round.</param>        
        private void SyncRoundData(RoundData roundData) {
            // Update data by removing empty lanes until the number of lanes matches the event data's lane count.
            for (int i = roundData.Count - 1; i >= 0; i--) {
                if (roundData.Count <= this.EventData.LaneCount) break;
                if (roundData[i].Players.Count != 0) continue;
                roundData.RemoveAt(i);
            }

            // If the number of lanes is less than the event data's lane count, add new empty lanes.
            while (roundData.Count < this.EventData.LaneCount) {
                roundData.Add(new MatchData(this.EventData.MatchFormat) {
                    Lane = roundData.Count + 1
                });
            }

            // Ensure all match data lanes are set correctly.
            for (int i = 0; i < roundData.Count; i++) {
                roundData[i].Lane = i;
            }

            // Change unplayed match ends to the event's default ends value.
            foreach (MatchData matchData in roundData) {
                if (matchData.Score.Sum() != 0) continue; // If the match has a score, it is played.
                matchData.Ends = this.EventData.DefaultEnds;
            }

            // Ensure all empty match data has the same match format as the event.
            foreach (MatchData matchData in roundData) {
                if (matchData.Players.Count != 0) continue;
                matchData.MatchFormat = this.EventData.MatchFormat;
            }
        }

        /// <summary>
        /// Fill matchRow cards with values from "roundRow".
        /// Clears all matchRow cards that does not have a value in "roundRow".
        /// </summary>
        /// <param name="roundRow"></param>
        private void PopulateMatchCards() {
            this.MatchCardStackPanel.Children.Clear();

            foreach(MatchData matchData in this.RoundData) {
                MatchCard matchCard = MatchCardFactory.GenerateMatchCard(matchData.MatchFormat);
                matchCard.Lane = matchData.Lane;
                matchCard.Ends = matchData.Ends;
                this.MatchCardStackPanel.Children.Add(matchCard);

                matchCard.Loaded += (s, e) => {
                    foreach (PlayerData player in matchData.Players) {
                        TeamCard teamCard = matchCard.GetTeamCard(player.Team)!;
                        teamCard.AddName(player.Name);
                    }

                    for (int i = 0; i < matchData.Score.Length; i++) {
                        TeamCard teamCard = matchCard.GetTeamCard(i)!;
                        teamCard.Bowls = matchData.Score[i];
                    }

                    if (matchData.TieBreaker >= 0) {
                        matchCard.SetTieBreaker(matchData.TieBreaker);
                    }
                };

                foreach (TeamCard teamCard in matchCard.Descendants<TeamCard>()) {
                    teamCard.TeamIndex = teamCard.TeamIndex;
                    teamCard.Bowls = matchData.Score[teamCard.TeamIndex];
                }
            }
        }
    }
}
