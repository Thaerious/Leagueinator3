using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Windows.Controls;
using static Leagueinator.GUI.Controllers.DragDropDelegates;

namespace Leagueinator.GUI.Controls {
    public class TeamCard : Border {

        public TeamCard() {
            this.AllowDrop = true;
            this.Loaded += this.HndLoaded;
        }

        #region Properties

        public BowlsPanel BowlsPanel => this.Ancestors<BowlsPanel>().First();

        public MatchCard MatchCard => this.Ancestors<MatchCard>().First();

        public static readonly DependencyProperty TeamIndexProperty =
            DependencyProperty.Register(nameof(TeamIndex), typeof(int), typeof(TeamCard));

        public int TeamIndex {
            get => (int)GetValue(TeamIndexProperty);
            set => SetValue(TeamIndexProperty, value);
        }

        public static readonly DependencyProperty BowlsProperty =
            DependencyProperty.Register(nameof(Bowls), typeof(int), typeof(TeamCard));

        public int Bowls {
            get => (int)GetValue(BowlsProperty);
            set => SetValue(BowlsProperty, value);
        }

        #endregion

        private void HndLoaded(object sender, RoutedEventArgs e) {
            DragDropController controller = new DragDropController(this);

            this.PreviewMouseDown += controller.HndPreMouseDown;
            this.DragEnter += controller.HndDragEnter;
            this.Drop += controller.HndDrop;

            this.GetDescendantsOfType<TextBox>()
                .Where(textBox => textBox.HasTag("PlayerName"))
                .ToList()
                .ForEach(textBox => {
                    textBox.AllowDrop = true;
                    textBox.DragEnter += controller.HndDragEnter;
                    textBox.PreviewDragOver += controller.HndPreviewDragOver;
                });
        }

        /// <summary>
        /// Determines whether any <see cref="TextBox"/> within the current object
        /// contains the specified name as its text.
        /// </summary>
        /// <param name="name">The name To search for within the text of <see cref="TextBox"/> controls.</param>
        /// <returns>
        /// <c>true</c> if any <see cref="TextBox"/> contains the specified name as its text;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method searches through all <see cref="TextBox"/> descendants of the current object
        /// and returns <c>true</c> if a match is found.
        /// </remarks>
        public bool HasName(string name) {
            return this.GetDescendantsOfType<TextBox>()
                       .Where(textBox => textBox.HasTag("PlayerName"))
                       .Where(textBox => textBox.Text == name)
                       .Any();
        }

        public void SetName(string name, int position) {
            TextBox[] boxes = this.FindByTag("PlayerName").OfType<TextBox>().ToArray();
            boxes[position].Text = name;
        }

        /// <summary>
        /// Removes the specified name from all <see cref="TextBox"/> controls that are descendants
        /// of the current object. If a <see cref="TextBox"/> contains the specified name, 
        /// its text is cleared.
        /// </summary>
        /// <param name="name">The name To remove from the text boxes.</param>
        public void RemoveName(string name) {
            this.GetDescendantsOfType<TextBox>()
                .Where(textBox => textBox.HasTag("PlayerName"))
                .Where(textBox => textBox.Text == name)
                .ToList()
                .ForEach(textBox => {
                    textBox.Clear();
                });
        }

        /// <summary>
        /// Clears the text from all <see cref="TextBox"/> controls that are descendants
        /// of the current object. 
        /// </summary>
        public void ClearNames() {
            this.GetDescendantsOfType<TextBox>()
                .Where(textBox => textBox.HasTag("PlayerName"))
                .ToList()
                .ForEach(textBox => {
                    textBox.Clear();
                });
        }
    }
}
