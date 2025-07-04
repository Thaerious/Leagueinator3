using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls {
    /// <summary>
    /// Interaction logic for InfoCard.xaml
    /// </summary>
    public partial class InfoCard : UserControl {
        public InfoCard() {
            this.AllowDrop = true
            this.Loaded += this.HndLoaded;;
            this.InitializeComponent();
        }

        public int Lane {
            get => int.Parse(this.LblLane.Text) - 1;
            set => this.LblLane.Text = (value + 1).ToString();
        }

        #region Handles

        private void HndLoaded(object sender, RoutedEventArgs e) {
            DragDropController controller = new DragDropController(this);

            this.PreviewMouseDown += controller.HndPreMouseDown;
            this.DragEnter += controller.HndDragEnter;
            this.Drop += controller.HndDrop;
            this.TxtEnds.PreviewTextInput += InputHandlers.OnlyNumbers;

            this.GetDescendantsOfType<TextBox>()
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
