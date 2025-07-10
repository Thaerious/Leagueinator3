using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

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

        [NamedEventHandler(EventName.EventRecordChanged)]
        internal void DoEventNameChanged(EventRecord eventRecord) {
            var index = EventRecords.ToList().FindIndex(r => r.UID == eventRecord.UID);
            if (index >= 0) EventRecords[index] = eventRecord;
        }

        private void HndDelete(object sender, EventArgs e) {
            this.NamedEventDisp.Dispatch(EventName.DeleteEvent, new() {
                ["eventUID"] = (this.EventData.SelectedItem as EventRecord)!.UID,
            });
        }

        private void HndExit(object sender, EventArgs e) {
            this.Close();
        }

        private void CellChanged(object sender, EventArgs e) {
            Debug.WriteLine($"Cell Event '{EventData.SelectedItem}'");

            //if (this.EventData.SelectedItem is EventItem item) {
            //    this.SelectedEvent = (EventItem)EventData.SelectedItem;
            //    this.ButDelete.IsEnabled = true;
            //}
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.EventData.SelectedItem is EventRecord record) {
                this.ButDelete.IsEnabled = true;
                this.NamedEventDisp.Dispatch(EventName.SelectEvent, new() {
                    ["uid"] = record.UID,
                });
            }
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

        private void ClickRename(object sender, RoutedEventArgs e) {

        }
    }
}
