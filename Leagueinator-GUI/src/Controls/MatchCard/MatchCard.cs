using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Leagueinator.GUI.Controls {

    public abstract partial class MatchCard : UserControl {
        public bool SuppressBowlsEvent = false;

        public abstract MatchFormat MatchFormat { get; }

        public MatchCard() {
            this.MouseDown += this.MatchCard_MouseDown;
            this.Loaded += MatchCard_Loaded;
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

            infoCard.TxtEnds.TextChanged += this.EndsTextChanged;
            infoCard.TxtEnds.PreviewMouseLeftButtonDown += this.TextBoxPreventDefaultCaretBehaviour;
            infoCard.TxtEnds.GotKeyboardFocus += this.TextBoxSelectAllOnFocus;

            foreach (TextBox textBox in this.FindByTag("Bowls").Cast<TextBox>()) {
                textBox.TextChanged += this.BowlsTextChanged;
                textBox.PreviewMouseLeftButtonDown += this.TextBoxPreventDefaultCaretBehaviour;
                textBox.GotKeyboardFocus += this.TextBoxSelectAllOnFocus;
            }
        }

        private void EndsTextChanged(object sender, TextChangedEventArgs e) {
            if (sender is not TextBox textBox) return;

            if (textBox.Text.Trim() == "") {
                textBox.Text = "0";
                textBox.SelectAll(); 
                return;
            }

            this.DispatchNamedEvent(EventName.ChangeEnds, new() {
                ["lane"] = this.Lane,
                ["ends"] = int.Parse(textBox.Text)
            });
        }

        private void BowlsTextChanged(object sender, TextChangedEventArgs e) {
            if (sender is not TextBox textBox) return;
            if (this.SuppressBowlsEvent) return; // Prevents looping when setting ChangeBowls property            

            if (textBox.Text.Trim() == "") {
                textBox.Text = "0"; 
                textBox.SelectAll(); 
                return;
            }

            this.DispatchNamedEvent(EventName.ChangeBowls, new() {
                ["lane"]      = this.Lane,
                ["bowls"]     = int.Parse(textBox.Text),
                ["teamIndex"] = textBox.Ancestors<TeamCard>().First().TeamIndex
            });
        }

        private void TextBoxPreventDefaultCaretBehaviour(object sender, MouseButtonEventArgs e) {
            if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin) {
                e.Handled = true; // Prevents default focus/caret behavior
                textBox.Focus();  // Triggers GotKeyboardFocus
            }
        }

        private void TextBoxSelectAllOnFocus(object sender, KeyboardFocusChangedEventArgs e) {
            (sender as TextBox)?.SelectAll();
        }

        public void SetEnds(int ends) {
            var infoCard = this.GetDescendantsOfType<InfoCard>().First();
            infoCard.TxtEnds.Text = ends.ToString();
        }

        public void SetBowls(int[] bowlsScored) {
            int i = 0;
            TextBox[] bowlsTextBoxes = this.FindByTag("Bowls").Cast<TextBox>().ToArray();
            if (bowlsTextBoxes.Length != bowlsScored.Length) throw new Exception("Array length mismatch between score and textboxes.");

            foreach (TextBox textBox in bowlsTextBoxes) {
                textBox.Text = bowlsScored[i++].ToString();
            }
        }

        public int Lane {
            get {
                var infoCard = this.GetDescendantsOfType<InfoCard>().FirstOrDefault();
                return infoCard?.Lane ?? throw new NullReferenceException("InfoCard not found.");
            }
            set {
                var infoCard = this.GetDescendantsOfType<InfoCard>().FirstOrDefault() 
                            ?? throw new NullReferenceException("InfoCard not found.");
                infoCard.Lane = value;
            }
        }

        private void MatchCard_MouseDown(object sender, MouseButtonEventArgs e) {
            var infoCard = this.GetDescendantsOfType<InfoCard>().First();
            var textBlock = infoCard.GetDescendantsOfType<TextBlock>().Where(tb => tb.Name == "LblLane").First();
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
            Debug.WriteLine($"HndRemoveMatch({this.Lane})");
            this.DispatchNamedEvent(EventName.RemoveMatch, new() {
                ["lane"] = this.Lane,
            });
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

            this.DispatchNamedEvent(EventName.ChangeMatchFormat, new() {
                ["lane"] = this.Lane,
                ["format"] = matchFormat
            });
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

            var checkTie = this.FindByTag("CheckTie").OfType<CheckBox>().ToList();
            if (checkTie.Count != 2) return;

            int clickedIndex = checkTie.IndexOf(checkBox);
            int otherIndex = 1 - clickedIndex;

            if (checkBox.IsChecked == true) checkTie[otherIndex].IsChecked = false;

            int newCheckedTeamIndex = checkTie.FindIndex(cb => cb.IsChecked == true);

            this.DispatchNamedEvent(EventName.ChangeTieBreaker, new() {
                ["lane"] = this.Lane,
                ["tieBreaker"] = newCheckedTeamIndex,
            });
        }

        public void OnlyNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
