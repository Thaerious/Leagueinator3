using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Windows.Controls;
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

        private static readonly DependencyProperty BowlsProperty =
            DependencyProperty.Register(nameof(Bowls), typeof(int), typeof(TeamCard));

        public int Bowls {
            get => (int)GetValue(BowlsProperty);
            set => SetValue(BowlsProperty, value);
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

        #endregion
    }
}
