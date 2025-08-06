using Leagueinator.GUI.Controllers;
using System.Windows;
using System.Windows.Controls;
using Leagueinator.Utility.Extensions;
using Leagueinator.GUI.Controllers.DragDropManager;
using Leagueinator.GUI.Controllers.NamedEvents;

namespace Leagueinator.GUI.Controls.MatchCards {
    /// <summary>
    /// Interaction logic for InfoCard.xaml
    /// </summary>
    public partial class InfoCard : UserControl, IDragDrop {
        public InfoCard() {
            this.AllowDrop = true;
            this.InitializeComponent();
            this.Loaded += this.HndLoaded;
        }

        public int Lane {
            get => int.Parse(this.LblLane.Text) - 1;
            set => this.LblLane.Text = (value + 1).ToString();
        }

        public void DoDrop(object dragSource) {
            if (dragSource is not InfoCard infoCard) return;
            this.DispatchEvent(EventName.SwapMatches, new() {
                ["lane1"] = this.Lane,
                ["lane2"] = infoCard.Lane
            });
        }

        #region Handles

        private void HndLoaded(object sender, RoutedEventArgs e) {
            DragDropManager<InfoCard> controller = new (this);
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
