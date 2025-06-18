using Leagueinator.GUI.Model;
using System.Windows;

namespace Leagueinator.GUI.Dialogs {
    /// <summary>
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window {
        public SettingsDialog() {
            this.InitializeComponent();
        }

        public EventData EventData {
            get {
                return new EventData {
                    MatchFormat = (MatchFormat)this.ListFormats.SelectedIndex,
                    LaneCount = int.Parse(this.TxtLanes.Text),
                    DefaultEnds = int.Parse(this.TxtEnds.Text)
                };
            }

            set {
                this.TxtLanes.Text = value.LaneCount.ToString();
                this.TxtEnds.Text = value.DefaultEnds.ToString();

                switch (value.MatchFormat) {
                    case MatchFormat.VS1:
                        this.ListFormats.SelectedItem = this.ListFormats.Items[0];
                        break;
                    case MatchFormat.VS2:
                        this.ListFormats.SelectedItem = this.ListFormats.Items[1];
                        break;
                    case MatchFormat.VS3:
                        this.ListFormats.SelectedItem = this.ListFormats.Items[2];
                        break;
                    case MatchFormat.VS4:
                        this.ListFormats.SelectedItem = this.ListFormats.Items[3];
                        break;
                    case MatchFormat.A4321:
                        this.ListFormats.SelectedItem = this.ListFormats.Items[4];
                        break;
                }
            }
        }

        private void HndOkButtonClick(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
            this.Close();
        }

        private void HndCancelButtonClick(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
            this.Close();
        }
    }
}
