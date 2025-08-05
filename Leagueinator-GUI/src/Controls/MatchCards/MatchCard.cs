using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Extensions;

namespace Leagueinator.GUI.Controls.MatchCards {

    /// <summary>
    /// Abstract base class for a user control that represents a match card UI element.
    /// Handles match-related UI and dispatches named _events for interactions.
    /// </summary>
    public abstract partial class MatchCard : UserControl {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchCard"/> class.
        /// Sets up event handlers for text boxes and match UI elements.
        /// </summary>
        public MatchCard() {
            this.Loaded += (s, e) => {
                InfoCard infoCard = this.GetDescendantsOfType<InfoCard>().FirstOrDefault()
                                  ?? throw new NullReferenceException("InfoCard not found on MatchCard.");

                infoCard.TxtEnds.TextChanged += this.EndsTextChanged;
                infoCard.TxtEnds.PreviewMouseLeftButtonDown += this.PreventCaretBehaviour;
                infoCard.TxtEnds.GotKeyboardFocus += this.TextBoxSelectAll;

                foreach (TextBox textBox in this.FindByTag("Bowls").Cast<TextBox>()) {
                    textBox.TextChanged += this.BowlsTextChanged;
                    textBox.PreviewMouseLeftButtonDown += this.PreventCaretBehaviour;
                    textBox.GotKeyboardFocus += this.TextBoxSelectAll;
                }
            };
        }

        /// <summary>
        /// Prevents caret jumping when clicking a non-focused textbox.
        /// </summary>
        private void PreventCaretBehaviour(object sender, MouseButtonEventArgs e) {
            if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin) {
                e.Handled = true;
                textBox.Focus();
            }
        }

        /// <summary>
        /// Updates the tie breaker selection by checking one checkbox and unchecking the other.
        /// </summary>
        /// <param name="teamIndex">The team index to set as tie breaker.</param>
        internal void SetTieBreaker(int teamIndex) {
            foreach (CheckBox cb in this.FindByTag("CheckTie").Cast<CheckBox>()) {
                var teamIndexForCB = cb.Ancestors<TeamCard>().FirstOrDefault()?.TeamIndex;
                cb.IsChecked = teamIndexForCB == teamIndex;
            }
        }

        /// <summary>
        /// Handles changes to the "Ends" textbox.
        /// Dispatches a named event with the new ends value.
        /// </summary>
        private void EndsTextChanged(object sender, TextChangedEventArgs e) {
            if (sender is not TextBox textBox) return;
            if (textBox.PreventEmpty()) return;

            this.DispatchEvent(EventName.ChangeEnds, new() {
                ["lane"] = this.Lane,
                ["ends"] = int.Parse(textBox.Text)
            });
        }

        /// <summary>
        /// Handles changes to the bowls score textboxes.
        /// Dispatches a named event with the updated score.
        /// </summary>
        private void BowlsTextChanged(object sender, TextChangedEventArgs e) {
            if (sender is not TextBox textBox) return;
            if (textBox.PreventEmpty()) return;

            this.DispatchEvent(EventName.ChangeBowls, new() {
                ["lane"] = this.Lane,
                ["bowls"] = int.Parse(textBox.Text),
                ["teamIndex"] = textBox.Ancestors<TeamCard>().First().TeamIndex
            });
        }

        /// <summary>
        /// Selects all text in a textbox when it receives keyboard focus.
        /// </summary>
        private void TextBoxSelectAll(object sender, KeyboardFocusChangedEventArgs e) {
            (sender as TextBox)?.SelectAll();
        }

        /// <summary>
        /// Sets the ends value in the UI.
        /// </summary>
        /// <param name="ends">The number of ends to display.</param>
        public void SetEnds(int ends) {
            var infoCard = this.GetDescendantsOfType<InfoCard>().First();
            infoCard.TxtEnds.Text = ends.ToString();
        }

        /// <summary>
        /// Sets the bowls scores for the teams.
        /// </summary>
        /// <param name="bowlsScored">An array of scores to apply to the textbox fields.</param>
        /// <exception cref="ArgumentNullException">Thrown if input array is null.</exception>
        /// <exception cref="Exception">Thrown if array length does not match number of UI fields.</exception>
        public void SetBowls(int[] bowlsScored) {
            if (bowlsScored is null) throw new ArgumentNullException(nameof(bowlsScored));

            int i = 0;
            TextBox[] bowlsTextBoxes = this.FindByTag("Bowls").Cast<TextBox>().ToArray();
            if (bowlsTextBoxes.Length != bowlsScored.Length) throw new Exception("Array length mismatch between score and textboxes.");

            foreach (TextBox textBox in bowlsTextBoxes) {
                textBox.Text = bowlsScored[i++].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the lane value from the InfoCard.
        /// </summary>
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

        /// <summary>
        /// Gets the <see cref="TeamCard"/> for a given team index.
        /// </summary>
        /// <param name="teamIndex">The team index.</param>
        /// <returns>The matching <see cref="TeamCard"/>.</returns>
        /// <exception cref="NullReferenceException">If the card is not found.</exception>
        public TeamCard GetTeamCard(int teamIndex) {
            var iterator = this.Descendants().Where(d => d.GetType() == typeof(TeamCard)).Cast<TeamCard>();
            TeamCard teamCard = iterator.ToList()[teamIndex];
            return teamCard;
        }

        /// <summary>
        /// Handles the context menu action to remove a match.
        /// </summary>
        public void HndRemoveMatch(object _, RoutedEventArgs __) {
            this.DispatchEvent(EventName.RemoveMatch, new() {
                ["lane"] = this.Lane,
            });
        }

        /// <summary>
        /// Handles the context menu action to change the match format.
        /// Expects the sender to be a MenuItem with a string Tag representing a DefaultMatchFormat enum value.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown if tag is missing.</exception>
        /// <exception cref="ArgumentException">Thrown if tag is invalid.</exception>
        public void HndChangeFormat(object sender, RoutedEventArgs __) {
            if (sender is not MenuItem menuItem) return;

            if (menuItem.Tag is null) return;
            if (menuItem.Tag is not string customData) throw new NullReferenceException("Missing tag on context menu item");

            bool success = Enum.TryParse(customData, out MatchFormat matchFormat);
            if (!success) throw new ArgumentException("Error on tag on context menu item");

            this.DispatchEvent(EventName.ChangeMatchFormat, new() {
                ["lane"] = this.Lane,
                ["format"] = matchFormat
            });
        }

        /// <summary>
        /// Handles tie breaker checkbox changes.
        /// Ensures only one checkbox is checked and dispatches an update event.
        /// </summary>
        public void HndTieValueChanged(object sender, RoutedEventArgs _) {
            if (sender is not CheckBox checkBox) return;

            var checkTie = this.FindByTag("CheckTie").OfType<CheckBox>().ToList();
            if (checkTie.Count != 2) return;

            int clickedIndex = checkTie.IndexOf(checkBox);
            int otherIndex = 1 - clickedIndex;

            if (checkBox.IsChecked == true) checkTie[otherIndex].IsChecked = false;

            this.DispatchEvent(EventName.ChangeTieBreaker, new() {
                ["lane"] = this.Lane,
                ["teamIndex"] = checkTie.FindIndex(cb => cb.IsChecked == true)
            });
        }
    }
}
