﻿using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace Leagueinator.GUI.Controls {
    public class TeamCard : Border {

        public TeamCard() {
            this.AllowDrop = true;
            this.Loaded += this.HndLoaded;
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
            DragDropController controller = new DragDropController(this);

            this.PreviewMouseDown += this.HndRequestFocus;
            this.PreviewMouseDown += controller.HndPreMouseDown;
            this.DragEnter        += controller.HndDragEnter;
            this.Drop             += controller.HndDrop;

            foreach (TextBox textBox in this.FindByTag("PlayerName").Cast<TextBox>()) {
                textBox.KeyUp             += this.NameTextBoxChange;
                textBox.LostKeyboardFocus += this.DispatchChangeName;
                textBox.DragEnter         += controller.HndDragEnter;
                textBox.PreviewDragOver   += controller.HndPreviewDragOver;
                textBox.AllowDrop = true;
            }
        }

        private void HndRequestFocus(object sender, MouseButtonEventArgs e) {
            NamedEvent.Dispatch(EventName.RequestFocus, new() {
                ["lane"]      = this.MatchCard.Lane,
                ["teamIndex"] = this.TeamIndex,
                ["append"]    = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
            });
        }

        protected void DispatchChangeName(object sender, RoutedEventArgs e) {
            if (sender is not TextBox textBox) return;
            var parent = (StackPanel)textBox.Parent ?? throw new NullReferenceException("Parent is not a StackPanel.");

            NamedEvent.Dispatch(EventName.ChangePlayerName, new() {
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
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    textBox.MoveFocus(request);
                }
            }
        }

        #endregion
    }
}
