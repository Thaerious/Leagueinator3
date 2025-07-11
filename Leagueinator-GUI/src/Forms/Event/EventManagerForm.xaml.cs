using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using EventRecord = Leagueinator.GUI.Model.EventRecord;

namespace Leagueinator.GUI.Forms.Event {
    /// <summary>
    /// Interaction logic for EventManagerForm.xaml
    /// </summary>
    public partial class EventManagerForm : Window {

        public ObservableCollection<EventRecord> EventRecords { get; set; } = [];

        public NamedEventDispatcher NamedEventDisp { get; set; }

        public NamedEventReceiver NamedEventRcv { get; private set; }

        public EventManagerForm() {
            this.NamedEventDisp = new(this);
            this.NamedEventRcv = new(this);
            InitializeComponent();

            this.Loaded += (s, e) => {
                this.TxtEnds.PreviewTextInput += InputHandlers.OnlyNumbers;
                this.TxtLanes.PreviewTextInput += InputHandlers.OnlyNumbers;
            };
        }

        private void EventManagerForm_Loaded(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        public void ShowDialog(MainController mainController, List<EventRecord> eventRecords, EventRecord selected) {
            foreach (var record in eventRecords) this.EventRecords.Add(record);
            this.DataContext = this;
            this.DoEventChanged(selected);
            this.ShowDialog();
        }

        private void HndNew(object sender, EventArgs e) {
            this.NamedEventDisp.Dispatch(EventName.AddEvent);
        }

        [NamedEventHandler(EventName.EventAdded)]
        internal void DoEventAdded(EventRecord eventRecord) {
            this.EventRecords.Add(eventRecord);
        }


        [NamedEventHandler(EventName.EventChanged)]
        internal void DoEventChanged(EventRecord eventRecord) {
            this.NamedEventDisp.PauseEvents();
            this.EventData.SelectedItem = eventRecord;
            this.TxtName.Text = eventRecord.Name;
            this.TxtEnds.Text = eventRecord.DefaultEnds.ToString();
            this.TxtLanes.Text = eventRecord.LaneCount.ToString();
            this.NamedEventDisp.ResumeEvents();
        }

        [NamedEventHandler(EventName.EventDeleted)]
        internal void DoEventDeleted(int uid) {
            var index = EventRecords.ToList().FindIndex(r => r.UID == uid);
            EventRecords.RemoveAt(index);
        }

        [NamedEventHandler(EventName.EventRecordChanged)]
        internal void DoEventNameChanged(EventRecord eventRecord) {
            var index = EventRecords.ToList().FindIndex(r => r.UID == eventRecord.UID);
            if (index < 0) return;

            if (this.EventData.SelectedIndex == index) {
                EventRecords[index] = eventRecord;
                this.EventData.SelectedItem = eventRecord;
            }
            else {
                EventRecords[index] = eventRecord;
            }
        }

        private void HndDelete(object sender, EventArgs e) {
            this.NamedEventDisp.Dispatch(EventName.DeleteEvent, new() {
                ["eventUID"] = (this.EventData.SelectedItem as EventRecord)!.UID,
            });
        }

        private void HndExit(object sender, EventArgs e) {
            this.Close();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.EventData.SelectedItem is not EventRecord record) return;
            this.ButDelete.IsEnabled = true;
            this.NamedEventDisp.Dispatch(EventName.SelectEvent, new() {
                ["uid"] = record.UID,
            });
        }

        private void TxtChanged(object sender, TextChangedEventArgs args) {
            if (this.IsLoaded == false) return;

            this.NamedEventDisp.Dispatch(EventName.ChangeEventArg, new() {
                ["name"] = this.TxtName.Text,
                ["laneCount"] = int.Parse(this.TxtLanes.Text),
                ["ends"] = int.Parse(this.TxtEnds.Text),
            });
        }

        private void EventTypeChanged(object sender, SelectionChangedEventArgs e) {
        }

        private void MatchFormatChanged(object sender, SelectionChangedEventArgs e) {
        }
    }
}
