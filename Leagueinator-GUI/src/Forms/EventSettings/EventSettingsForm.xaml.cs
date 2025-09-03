using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls.MatchCards;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using System.Windows;

namespace Leagueinator.GUI.Forms.Event {

    public record FormatRecord<T>(T Format, string DisplayName);

    /// <summary>
    /// Interaction logic for EventSettingsForm.xaml
    /// </summary>
    public partial class EventSettingsForm : Window {
        public EventSettingsForm() : base() {
            InitializeComponent();

            this.TxtEnds.PreviewTextInput += InputHandlers.OnlyNumbers;
            this.TxtLanes.PreviewTextInput += InputHandlers.OnlyNumbers;

            this.ListMatchFormat.ItemsSource = new List<FormatRecord<MatchFormat>> {
                new(MatchFormat.VS1, MatchFormat.VS1.ToString()),
                new(MatchFormat.VS2, MatchFormat.VS2.ToString()),
                new(MatchFormat.VS3, MatchFormat.VS3.ToString()),
                new(MatchFormat.VS4, MatchFormat.VS4.ToString()),
                new(MatchFormat.A4321, MatchFormat.A4321.ToString()),
            };

            this.ListEventType.ItemsSource = new List<FormatRecord<EventType>> {
                new(EventType.Swiss, "Swiss"),
                new(EventType.Jitney, "Jitney"),
            };


            this.ListMatchScoring.ItemsSource = new List<FormatRecord<MatchScoring>> {
                new(MatchScoring.Bowls, "Bowls"),
                new(MatchScoring.Plus, "Plus"),
            };

            this.ListMatchFormat.SelectedValuePath = "Format";
            this.ListMatchFormat.DisplayMemberPath = "DisplayName";

            this.ListEventType.SelectedValuePath = "Format";
            this.ListEventType.DisplayMemberPath = "DisplayName";

            this.ListMatchScoring.SelectedValuePath = "Format";
            this.ListMatchScoring.DisplayMemberPath = "DisplayName";
        }

        public MatchFormat MatchFormat => (MatchFormat)this.ListMatchFormat.SelectedValue;

        public EventType EventType => (EventType)this.ListEventType.SelectedValue;

        public MatchScoring MatchScoring => (MatchScoring)this.ListMatchScoring.SelectedValue;

        public bool HeadToHead => this.CheckHeadToHead.IsChecked ?? false;

        public bool? ShowDialog(EventData eventData) {
            this.TxtName.Text = eventData.EventName;
            this.TxtLanes.Text = eventData.LaneCount.ToString();
            this.TxtEnds.Text = eventData.DefaultEnds.ToString();
            this.ListMatchFormat.SelectedValue = eventData.DefaultMatchFormat;
            this.ListEventType.SelectedValue = eventData.EventType;
            this.ListMatchScoring.SelectedValue = eventData.MatchScoring;
            this.CheckHeadToHead.IsChecked = eventData.HeadToHeadScoring;

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
