using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
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

        private EventItem? SelectedEvent = null;

        public EventManagerForm() {
            this.NamedEventDisp = new(this);
            this.NamedEventRcv = new(this);
            InitializeComponent();
        }

        public void ShowDialog(MainController mainController, List<EventRecord> eventRecords) {
            foreach (var record in eventRecords) this.EventRecords.Add(record);
            this.DataContext = this;
            this.ShowDialog();
        }

        private void HndNew(object sender, EventArgs e) {
            this.NamedEventDisp.Dispatch(EventName.AddEvent);
        }

        [NamedEventHandler(EventName.EventAdded)]
        internal void DoEventAdded(EventRecord eventRecord) {
            this.EventRecords.Add(eventRecord);
        }

        private void HndSelect(object sender, EventArgs e) {
            if (this.SelectedEvent is null) return;

            this.NamedEventDisp.Dispatch(EventName.SelectEvent, new() {
                ["eventUID"] = this.SelectedEvent.EventUID,
            });
        }

        private void HndDelete(object sender, EventArgs e) {
            if (this.SelectedEvent is null) return;

            this.NamedEventDisp.Dispatch(EventName.DeleteEvent, new() {
                ["eventUID"] = this.SelectedEvent.EventUID,
            });
        }

        private void HndExit(object sender, EventArgs e) {
            this.Close();
        }

        private void CellChanged(object sender, EventArgs e) {
            Debug.WriteLine($"Cell Event '{EventData.SelectedItem}'");

            if (this.EventData.SelectedItem is EventItem item) {
                this.SelectedEvent = (EventItem)EventData.SelectedItem;
                this.ButDelete.IsEnabled = true;
                this.ButSelect.IsEnabled = true;
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Debug.WriteLine($"Selection Event '{EventData.SelectedItem}'");

            if (this.EventData.SelectedItem is EventItem item) {
                this.SelectedEvent = (EventItem)EventData.SelectedItem;
                this.ButDelete.IsEnabled = true;
                this.ButSelect.IsEnabled = true;
            }
        }

        private void ClickRename(object sender, RoutedEventArgs e) {

        }
    }
}
