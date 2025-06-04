using Leagueinator.GUI.Utility.Extensions;
using Leagueinator.GUI.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Leagueinator.GUI.Controls.MemoryTextBox;

namespace Leagueinator.GUI.Controls {
    public class MatchCardFormatArgs(RoutedEvent routedEvent, MatchCard matchCard, MatchFormat matchFormat) : RoutedEventArgs(routedEvent) {
        public readonly MatchCard MatchCard = matchCard;
        public readonly MatchFormat MatchFormat = matchFormat;
    }

    public abstract class MatchCard : UserControl {
        public MatchCard() {
            // Add a routed handler which bubbles events from child components.
            this.AddHandler(RegisteredUpdateEvent, new MemoryEventHandler(this.HndUpdatePlayerText));
            //this.LayoutUpdated += this.MatchCard_LayoutUpdated;
            this.MouseDown += this.MatchCard_MouseDown;
        }

        private void MatchCard_MouseDown(object sender, MouseButtonEventArgs e) {
            var infoCard = this.Descendants<InfoCard>().First();
            var textBlock = infoCard.Descendants<TextBlock>().Where(tb => tb.Name == "LblLane").First();
        }

        public delegate void FormatChangedEventHandler(object sender, MatchCardFormatArgs e);

        public static readonly RoutedEvent RegisteredFormatChangedEvent = EventManager.RegisterRoutedEvent(
            "FormatChanged",                      // Event name
            RoutingStrategy.Bubble,               // Routing strategy (Bubble, Tunnel, or Direct)
            typeof(FormatChangedEventHandler),    // Delegate type
            typeof(MatchCard)                     // Owner type
        );

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

        /// <summary>
        /// Handlers for when the MatchFormat changes.
        /// The change occurs in the context menu.
        /// </summary>
        public event FormatChangedEventHandler FormatChanged {
            add { this.AddHandler(RegisteredFormatChangedEvent, value); }
            remove { this.RemoveHandler(RegisteredFormatChangedEvent, value); }
        }

        public TeamCard? GetTeamCard(int teamIndex) {
            return this
                .Descendants<TeamCard>()
                .Where(teamCard => teamCard.TeamIndex == teamIndex)
                .FirstOrDefault();
        }

        /// <summary>
        /// Context menu handler for remove match.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HndRemoveMatch(object _, RoutedEventArgs __) {
            // TODO: Implement match removal logic.
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

            // TODL: Raise the format changed event with the new match format.
        }

        /// <summary>
        /// Handles the changing of a MemoryTextBox text value;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NullReferenceException"></exception>
        private void HndUpdatePlayerText(object sender, MemoryTextBoxArgs e) {
            string prev = e.Before?.Trim() ?? "";
            string after = e.After?.Trim() ?? "";
            int teamIndex = e.TextBox.Ancestors<TeamCard>().First().TeamIndex;

            // If there is no change, focus next component and terminate.
            if (after == prev) {
                var focusedElement = Keyboard.FocusedElement as UIElement;
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                focusedElement?.MoveFocus(request);
                return;
            }

            // If enter initiated the event, focus next textbox.
            if (e.Cause == Cause.EnterPressed) {
                StackPanel stackPanel = (StackPanel)e.TextBox.Parent;
                int index = stackPanel.Children.IndexOf(e.TextBox);
                if (index + 1 < stackPanel.Children.Count) {
                    stackPanel.Children[index + 1].Focus();
                }
                e.TextBox.SelectAll();
            }

            // TODO: Implement event logic to update the MatchRow with the new player name.
        }

        /// <summary>
        /// Invoked when a tie checkbox matchRow changes.
        /// Will uncheck the other tie check box on this match card.
        /// Updates the underlying TeamRow matchRow for tie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void HndTieValueChanged(object sender, RoutedEventArgs _) {
            if (sender is not CheckBox checkBox) return;

            CheckBox checkTie0 = this.FindName("CheckTie0") as CheckBox ?? throw new NullReferenceException();
            CheckBox checkTie1 = this.FindName("CheckTie1") as CheckBox ?? throw new NullReferenceException();

            // Uncheck the opposite checkbox.
            if (checkBox.IsChecked == true && checkBox == checkTie0) {
                checkTie1.IsChecked = false;
            }
            else if (checkBox.IsChecked == true && checkBox == checkTie1) {
                checkTie0.IsChecked = false;
            }

            // Set model matchRow according To which box is checked.            
            if (checkTie0.IsChecked == true && checkTie1.IsChecked == false) {
                //this.MatchRow.Teams[0]!.Tie = true;
                //this.MatchRow.Teams[1]!.Tie = false;
            }
            else if (checkTie0.IsChecked == false && checkTie1.IsChecked == true) {
                //this.MatchRow.Teams[0]!.Tie = false;
                //this.MatchRow.Teams[1]!.Tie = true;
            }
            else {
                //this.MatchRow.Teams[0]!.Tie = false;
                //this.MatchRow.Teams[1]!.Tie = false;
            }
        }

        new private bool IsLoaded = false;
    }
}
