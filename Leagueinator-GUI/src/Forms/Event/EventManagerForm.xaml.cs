using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls.MatchCards;
using Leagueinator.GUI.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using EventRecord = Leagueinator.GUI.Model.EventRecord;

namespace Leagueinator.GUI.Forms.Event {

    public record MatchFormatRecord {
        public MatchFormatRecord(MatchFormat MatchFormat, string DisplayName) {
            this.MatchFormat = MatchFormat;
            this.DisplayName = DisplayName;
        }

        public MatchFormat MatchFormat { get;}
        public string DisplayName { get; }
    }
    public record EventTypeRecord{
        public EventTypeRecord(EventType EventType, string DisplayName) {
            this.EventType = EventType;
            this.DisplayName = DisplayName;
        }
        public EventType EventType { get; }
        public string DisplayName { get; }
    }

    /// <summary>
    /// Interaction logic for EventManagerForm.xaml
    /// </summary>
    public partial class EventManagerForm : Window {

        public ObservableCollection<EventRecord> EventRecords { get; set; } = [];

        public EventManagerForm() {
            InitializeComponent();

            this.Loaded += (s, e) => {
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
            };
        }

        public void ShowDialog(MainController mainController, List<EventRecord> eventRecords, EventRecord selected) {
            foreach (var record in eventRecords) this.EventRecords.Add(record);
            this.DataContext = this;

            this.Loaded += (s, e) => {
                this.DoEventChanged(selected);
                this.ResumeEvents();
            };

            this.ShowDialog();            
        }

        #region Button Handlers

        private void HndNew(object sender, EventArgs e) {
            this.DispatchEvent(EventName.AddEvent);
        }

        private void HndExit(object sender, EventArgs e) {
            this.Close();
        }

        #endregion

        #region Named Event Handlers

        [NamedEventHandler(EventName.EventAdded)]
        internal void DoEventAdded(EventRecord eventRecord) {
            this.EventRecords.Add(eventRecord);
        }


        [NamedEventHandler(EventName.EventSelected)]
        internal void DoEventChanged(EventRecord eventRecord) {
            this.PauseEvents();
            this.EventData.SelectedItem = eventRecord;
            this.TxtName.Text = eventRecord.Name;
            this.TxtEnds.Text = eventRecord.DefaultEnds.ToString();
            this.TxtLanes.Text = eventRecord.LaneCount.ToString();
            this.ListMatchFormat.SelectedValue = eventRecord.MatchFormat;
            this.ListEventType.SelectedValue = eventRecord.EventType;
            this.ResumeEvents();
        }

        #endregion

        #region Component Handlers
        private void TxtChanged(object sender, TextChangedEventArgs args) {
            this.InvokeChangeEventArg();
        }

        private void EventTypeChanged(object sender, SelectionChangedEventArgs e) {
            if (this.IsLoaded == false) return;

            EventTypeRecord etf = (EventTypeRecord)this.ListEventType.SelectedItem;
            if (etf is null) return;

            this.DispatchEvent(EventName.ChangeEventType, new() {
                ["eventType"] = etf.EventType
            });
        }

        private void MatchFormatChanged(object sender, SelectionChangedEventArgs e) {
            this.InvokeChangeEventArg();
        }
        #endregion

        private void InvokeChangeEventArg() {
            if (this.IsLoaded == false) return;

            MatchFormatRecord? mfr = (MatchFormatRecord)this.ListMatchFormat.SelectedItem;
            if (mfr is null) return;

            this.DispatchEvent(EventName.ChangeEventArg, new() {
                ["name"] = this.TxtName.Text,
                ["laneCount"] = int.Parse(this.TxtLanes.Text),
                ["ends"] = int.Parse(this.TxtEnds.Text),
                ["matchFormat"] = mfr.MatchFormat
            });
        }
    }
}
