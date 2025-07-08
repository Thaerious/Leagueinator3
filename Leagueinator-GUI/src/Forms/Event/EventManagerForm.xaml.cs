using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Forms.Event {
    /// <summary>
    /// Interaction logic for EventManagerForm.xaml
    /// </summary>
    public partial class EventManagerForm : Window {

        public NamedEventDispatcher NamedEventDisp { get; set; }

        public NamedEventReceiver NamedEventRcv { get; private set; }

        private EventItem? SelectedEvent = null;

        public EventManagerForm() {
            this.NamedEventDisp = new(this);
            this.NamedEventRcv = new(this);
            InitializeComponent();
        }

        public void ShowDialog(MainController mainController, IReadOnlyCollection<ReadOnlyEventData> data) {
            List<EventItem> list = [];

            foreach (ReadOnlyEventData item in data) {
                list.Add(new EventItem(mainController) {
                    EventUID = item.UID,
                    Name = item.EventName,
                    Date = item.Date,
                    Rounds = item.Rounds.Count
                });
            }

            this.EventData.ItemsSource = list;
            this.ShowDialog();
        }

        private void HndNew(object sender, EventArgs e) {
            this.NamedEventDisp.Dispatch(EventName.AddEvent);
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
                this.TxtEventName.Text = item.Name;
                this.ButDelete.IsEnabled = true;
                this.ButSelect.IsEnabled = true;
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Debug.WriteLine($"Selection Event '{EventData.SelectedItem}'");

            if (this.EventData.SelectedItem is EventItem item) {
                this.SelectedEvent = (EventItem)EventData.SelectedItem;
                this.TxtEventName.Text = item.Name;
                this.ButDelete.IsEnabled = true;
                this.ButSelect.IsEnabled = true;
            }
        }

        private void ClickRename(object sender, RoutedEventArgs e) {

        }
    }
}
