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
                    MatchFormat = (MatchFormat)this.ListMatchFormat.SelectedIndex,
                    LaneCount = int.Parse(this.TxtLanes.Text),
                    DefaultEnds = int.Parse(this.TxtEnds.Text),
                    EventType = (EventType)this.ListEventType.SelectedIndex
                };
            }

            set {
                this.TxtLanes.Text = value.LaneCount.ToString();
                this.TxtEnds.Text = value.DefaultEnds.ToString();

                switch(value.EventType) {
                    case EventType.RankedLadder:
                        this.ListEventType.SelectedItem = this.ListEventType.Items[0];
                        break;
                    case EventType.RoundRobin:
                        this.ListEventType.SelectedItem = this.ListEventType.Items[1];
                        break;
                    case EventType.Motley:
                        this.ListEventType.SelectedItem = this.ListEventType.Items[2];
                        break;
                }

                switch (value.MatchFormat) {
                    case MatchFormat.VS1:
                        this.ListMatchFormat.SelectedItem = this.ListMatchFormat.Items[0];
                        break;
                    case MatchFormat.VS2:
                        this.ListMatchFormat.SelectedItem = this.ListMatchFormat.Items[1];
                        break;
                    case MatchFormat.VS3:
                        this.ListMatchFormat.SelectedItem = this.ListMatchFormat.Items[2];
                        break;
                    case MatchFormat.VS4:
                        this.ListMatchFormat.SelectedItem = this.ListMatchFormat.Items[3];
                        break;
                    case MatchFormat.A4321:
                        this.ListMatchFormat.SelectedItem = this.ListMatchFormat.Items[4];
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
