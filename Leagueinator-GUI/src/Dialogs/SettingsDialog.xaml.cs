using Leagueinator.GUI.Forms.Main;
using System.Windows;
using System.Windows.Input;

namespace Leagueinator.GUI.Dialogs {
    /// <summary>
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window {
        public SettingsDialog(EventData eventData) {
            this.InitializeComponent();
            this.TxtLanes.Text = eventData.LaneCount.ToString();
            this.TxtEnds.Text = eventData.DefaultEnds.ToString();

            switch (eventData.MatchFormat) {
                case Utility.MatchFormat.VS1:
                    this.ListFormats.SelectedItem = this.ListFormats.Items[0];
                    break;
                case Utility.MatchFormat.VS2:
                    this.ListFormats.SelectedItem = this.ListFormats.Items[1];
                    break;
                case Utility.MatchFormat.VS3:
                    this.ListFormats.SelectedItem = this.ListFormats.Items[2];
                    break;
                case Utility.MatchFormat.VS4:
                    this.ListFormats.SelectedItem = this.ListFormats.Items[3];
                    break;
                case Utility.MatchFormat.A4321:
                    this.ListFormats.SelectedItem = this.ListFormats.Items[4];
                    break;
            }
        }

        public EventData EventData {
            get {
                return new EventData() {
                    MatchFormat = (Utility.MatchFormat)this.ListFormats.SelectedIndex,
                    LaneCount = int.Parse(this.TxtLanes.Text),
                    DefaultEnds = int.Parse(this.TxtEnds.Text)
                };
            }
        }

        private void HndOkButtonClick(object sender, RoutedEventArgs e) {
            // Set the NewName property To the text in the textbox.
            
            this.DialogResult = true;
            this.Close();
        }

        private void HndCancelButtonClick(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
            this.Close();
        }
    }
}
