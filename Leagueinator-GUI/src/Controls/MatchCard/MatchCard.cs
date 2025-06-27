using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Leagueinator.GUI.Controls {

    public abstract partial class MatchCard : UserControl {
        private int? _pendingLane = null;
        private int? _pendingEnds = null;        
        private int _lastCheckedTeamIndex = -1;
        public bool SuppressBowlsEvent = false;

        public abstract MatchFormat MatchFormat { get; }

        public MatchCard() {
            this.MouseDown += this.MatchCard_MouseDown;
            this.Loaded += MatchCard_Loaded;               
        }

        private void NameTextBoxChange(object sender, System.Windows.RoutedEventArgs e) { 
            if (sender is not TextBox textBox) return;
            var parent = (StackPanel)textBox.Parent ?? throw new NullReferenceException("Parent is not a StackPanel.");            

            int teamIndex = textBox.Ancestors<TeamCard>().First().TeamIndex;
            int position = parent.Children.IndexOf(textBox);

            if (e is KeyEventArgs keyArgs) {
                if (keyArgs.Key == Key.Enter) {
                    this.InvokeEvent("Name", value: textBox.Text, team: teamIndex, pos: position);
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    textBox.MoveFocus(request);
                }
            }
            else {
                this.InvokeEvent("Name", value: textBox.Text, team: teamIndex, pos: position);
            }
        }

        internal void SetTieBreaker(int teamIndex) {
            foreach (CheckBox cb in this.FindByTag("CheckTie").Cast<CheckBox>()) {
                var teamIndexForCB = cb.Ancestors<TeamCard>().FirstOrDefault()?.TeamIndex;
                if (teamIndexForCB == teamIndex) {
                    cb.IsChecked = true;
                }
                else {
                    cb.IsChecked = false;
                }
            }
        }

        private void MatchCard_Loaded(object sender, RoutedEventArgs e) {
            InfoCard infoCard = this.GetDescendantsOfType<InfoCard>().FirstOrDefault() ?? throw new NullReferenceException("InfoCard not found in MatchCard.");

            if (_pendingLane.HasValue) {
                infoCard.LblLane.Text = _pendingLane.Value.ToString();
                _pendingLane = null;
            }

            if (_pendingEnds.HasValue) {
                infoCard.TxtEnds.Text = _pendingEnds.Value.ToString();
                _pendingEnds = null;
            }

            infoCard.TxtEnds.TextChanged += (s, args) => {
                var ends = int.Parse(infoCard.TxtEnds.Text);
                this.InvokeEvent("Ends", value: ends);
            };

            foreach (TextBox textBox in this.FindByTag("Bowls").Cast<TextBox>()) {
                textBox.TextChanged += (s, args) => {
                    if (this.SuppressBowlsEvent) return; // Prevents looping when setting Bowls property
                    int teamIndex = textBox.Ancestors<TeamCard>().First().TeamIndex;
                    var textBoxValue = int.Parse(textBox.Text);
                    this.InvokeEvent("Bowls", value:textBoxValue, team:teamIndex);
                };
            }

            foreach (TextBox textBox in this.FindByTag("PlayerName").Cast<TextBox>()) {
                textBox.KeyDown += this.NameTextBoxChange;
                textBox.LostFocus += this.NameTextBoxChange;
            }
        }

        public int Ends {
            get {
                var infoCard = this.GetDescendantsOfType<InfoCard>().FirstOrDefault();
                return infoCard?.Lane ?? this._pendingEnds ?? 0;
            }
            set {
                var infoCard = this.GetDescendantsOfType<InfoCard>().FirstOrDefault();
                if (infoCard != null) {
                    infoCard.TxtEnds.Text = value.ToString();
                }
                else {
                    this._pendingEnds = value;
                }
            }
        }

        public int Lane {
            get {
                var infoCard = this.GetDescendantsOfType<InfoCard>().FirstOrDefault();
                return infoCard?.Lane - 1 ?? this._pendingLane ?? - 1;
            }
            set {
                var infoCard = this.GetDescendantsOfType<InfoCard>().FirstOrDefault();

                if (infoCard != null) {
                    infoCard.Lane = value + 1;
                }
                else {
                    this._pendingLane = value + 1;
                }
            }
        }

        private void MatchCard_MouseDown(object sender, MouseButtonEventArgs e) {
            var infoCard = this.GetDescendantsOfType<InfoCard>().First();
            var textBlock = infoCard.GetDescendantsOfType<TextBlock>().Where(tb => tb.Name == "LblLane").First();
        }

        // Force update the binding source when Enter is pressed
        protected void TxtEnterPressedHnd(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (sender is TextBox textBox) {
                    // Trigger binding update
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                    // Move focus To the next control in the tab order
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    textBox.MoveFocus(request);
                }
            }
        }

        public TeamCard? GetTeamCard(int teamIndex) {
            return this
                .GetDescendantsOfType<TeamCard>()
                .Where(teamCard => teamCard.TeamIndex == teamIndex)
                .FirstOrDefault();
        }

        /// <summary>
        /// Context menu handler for remove match.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HndRemoveMatch(object _, RoutedEventArgs __) {
            this.InvokeEvent("Remove");
        }

        /// <summary>
        /// Context menu handler for changing the match format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void HndChangeFormat(object sender, RoutedEventArgs __) {
            if (sender is not MenuItem menuItem) return;

            if (menuItem.Tag is null) return;  // tag is null during initialization
            if (menuItem.Tag is not string customData) throw new NullReferenceException("Missing tag on context menu item");

            bool success = Enum.TryParse(customData, out MatchFormat matchFormat);
            if (!success) throw new ArgumentException("Error on tag on context menu item");
            
            this.InvokeEvent("Format", value: matchFormat);
        }

        public void UpdateData(MatchData matchData) {
            this.Lane = matchData.Lane;
            this.Ends = matchData.Ends;
            this.SetTieBreaker(matchData.TieBreaker);

            this.SuppressBowlsEvent = true;

            for (int team = 0; team < matchData.Teams.Length; team++) {
                for (int position = 0; position < matchData.Teams[team].Length; position++) {
                    TeamCard teamCard = this.GetTeamCard(team)!;
                    var name = matchData.Teams[team][position];
                    teamCard.SetName(name, position);
                    teamCard.Bowls = matchData.Score[team];
                }
            }

            this.SuppressBowlsEvent = false;
        }

        /// <summary>
        /// Invoked when a tie checkBox matchRow changes.
        /// Will uncheck the other tie check box on this match card.
        /// Updates the underlying TeamRow matchRow for tie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HndTieValueChanged(object sender, RoutedEventArgs _) {
            if (sender is not CheckBox checkBox) return;

            List<CheckBox> checkTie = this.FindByTag("CheckTie")
                                          .OfType<CheckBox>()
                                          .ToList();

            // Uncheck the opposite checkBox.
            if (checkBox.IsChecked == true && checkBox == checkTie[0]) {
                checkTie[1].IsChecked = false;
            }
            else if (checkBox.IsChecked == true && checkBox == checkTie[1]) {
                checkTie[0].IsChecked = false;
            }

            int newCheckedTeamIndex = -1;

            if (checkTie[0].IsChecked == true && checkTie[1].IsChecked == false) {
                newCheckedTeamIndex = 0;
            }
            else if (checkTie[0].IsChecked == false && checkTie[1].IsChecked == true) {
                newCheckedTeamIndex = 1;
            }
            else {
                newCheckedTeamIndex = -1;
            }

            int lastCheckedTeamIndex = _lastCheckedTeamIndex;
            this._lastCheckedTeamIndex = newCheckedTeamIndex;
            this.InvokeEvent("Tie", value: newCheckedTeamIndex);
        }

        public void OnlyNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text) {
            return text.All(char.IsDigit);
        }
    }
}
