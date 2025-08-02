using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls.MatchCards;
using Leagueinator.GUI.Model;
using System.Diagnostics;
using System.Windows;

namespace Leagueinator.GUI.Forms.Event {

    public record MatchFormatRecord {
        public MatchFormatRecord(MatchFormat MatchFormat, string DisplayName) {
            this.MatchFormat = MatchFormat;
            this.DisplayName = DisplayName;
        }

        public MatchFormat MatchFormat { get; }
        public string DisplayName { get; }
    }
    public record EventTypeRecord {
        public EventTypeRecord(EventType EventType, string DisplayName) {
            this.EventType = EventType;
            this.DisplayName = DisplayName;
        }
        public EventType EventType { get; }
        public string DisplayName { get; }
    }

    /// <summary>
    /// Interaction logic for EventSettingsForm.xaml
    /// </summary>
    public partial class EventSettingsForm : Window {
        public EventSettingsForm() : base() {
            InitializeComponent();

            this.TxtEnds.PreviewTextInput += InputHandlers.OnlyNumbers;
            this.TxtLanes.PreviewTextInput += InputHandlers.OnlyNumbers;

            this.ListMatchFormat.ItemsSource = new List<MatchFormatRecord> {
                    new(MatchFormat.VS1, MatchFormat.VS1.ToString()),
                    new(MatchFormat.VS2, MatchFormat.VS2.ToString()),
                    new(MatchFormat.VS3, MatchFormat.VS3.ToString()),
                    new(MatchFormat.VS4, MatchFormat.VS4.ToString()),
                    new(MatchFormat.A4321, MatchFormat.A4321.ToString()),
                };

            this.ListEventType.ItemsSource = new List<EventTypeRecord> {
                    new(EventType.RankedLadder, "Ranked Ladder"),
                    new(EventType.Motley, "Motley"),
                    //new(EventType.RoundRobin, "Round Robin"), TODO ENABLE
                };

            this.ListMatchFormat.SelectedValuePath = "MatchFormat";
            this.ListMatchFormat.DisplayMemberPath = "DisplayName";

            this.ListEventType.SelectedValuePath = "EventType";
            this.ListEventType.DisplayMemberPath = "DisplayName";

        }

        public MatchFormat MatchFormat => (MatchFormat)this.ListMatchFormat.SelectedValue;

        public bool? ShowDialog(EventData eventData) {
            Debug.WriteLine("SHOW");
            this.TxtName.Text = eventData.EventName;
            this.TxtLanes.Text = eventData.LaneCount.ToString();
            this.TxtEnds.Text = eventData.DefaultEnds.ToString();
            this.ListMatchFormat.SelectedValue = eventData.MatchFormat;
            this.ListEventType.SelectedValue = eventData.EventType;

            this.ResumeEvents();
            return this.ShowDialog();
        }

        private void HndOk(object sender, RoutedEventArgs e) {
            this.DialogResult = true;   
            this.Close();
        }
        private void HndCancel(object sender, RoutedEventArgs e) {
            this.DialogResult = false;  
            this.Close();
        }
    }
}
