using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.DragDropManager;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace Leagueinator.GUI.Controls.MatchCards {
    public class TeamCard : Border, IDragDrop {

        public TeamCard() {
            this.AllowDrop = true;
            this.Loaded += this.HndLoaded;

            this.LostFocus += this.TeamCard_LostFocus;
        }

        private void TeamCard_LostFocus(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is TextBox textBox) {
                this.DispatchChangeName(textBox);
            }
        }

        #region Properties

        public MatchCard MatchCard => this.Ancestors<MatchCard>().First();

        private static readonly DependencyProperty TeamIndexProperty =
            DependencyProperty.Register(nameof(TeamIndex), typeof(int), typeof(TeamCard));

        public int TeamIndex {
            get => (int)GetValue(TeamIndexProperty);
            set => SetValue(TeamIndexProperty, value);
        }

        public string this[int i] {
            get {
                TextBox[] boxes = [.. this.FindByTag("PlayerName").OfType<TextBox>()];
                return boxes[i].Text;
            }
            set {
                TextBox[] boxes = [.. this.FindByTag("PlayerName").OfType<TextBox>()];
                boxes[i].Text = value;
            }
        }

        #endregion

        #region Handles

        private void HndLoaded(object sender, RoutedEventArgs e) {
            DragDropManager<TeamCard> controller = new (this);

            foreach (TextBox textBox in this.FindByTag("PlayerName").Cast<TextBox>()) {
                textBox.KeyUp             += this.NameTextBoxChange;
                textBox.LostKeyboardFocus += this.DispatchChangeName;
                textBox.DragEnter         += controller.HndDragEnter;
                textBox.PreviewDragOver   += controller.HndPreviewDragOver;
                textBox.AllowDrop = true;
            }
        }

        protected void DispatchChangeName(object sender, RoutedEventArgs? _ = null) {
            if (sender is not TextBox textBox) return;
            var parent = (StackPanel)textBox.Parent ?? throw new NullReferenceException("Parent is not a StackPanel.");

            this.DispatchEvent(EventName.ChangePlayerName, new() {
                ["lane"]      = this.MatchCard.Lane,
                ["name"]      = textBox.Text,
                ["teamIndex"] = textBox.Ancestors<TeamCard>().First().TeamIndex,
                ["position"]  = parent.Children.IndexOf(textBox)
            });
        }

        protected void NameTextBoxChange(object sender, RoutedEventArgs e) {
            if (sender is not TextBox textBox) return;

            if (e is KeyEventArgs keyArgs) {
                if (keyArgs.Key == Key.Enter) {
                    this.DispatchChangeName(sender, e);

                    // Focus the next element.
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    textBox.MoveFocus(request);
                }
            }
        }

        public void DoDrop(object dragSource) {
            if (dragSource == this) return;
            if (dragSource is TeamCard that) {
                this.DispatchEvent(EventName.SwapTeams, new() {
                    ["fromLane"]  = that.MatchCard.Lane,
                    ["toLane"]    = this.MatchCard.Lane,
                    ["fromIndex"] = that.TeamIndex,
                    ["toIndex"]   = this.TeamIndex,
                });
            }
        }

        #endregion
    }
}
