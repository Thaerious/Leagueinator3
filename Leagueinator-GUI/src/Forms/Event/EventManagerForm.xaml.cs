using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Model;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Forms.Event {
    /// <summary>
    /// Interaction logic for EventManagerForm.xaml
    /// </summary>
    public partial class EventManagerForm : Window {
        public EventManagerForm() {
            InitializeComponent();
        }

        public void ShowDialog(MainController mainController, LeagueData data) {
            List<EventItem> list = [];

            foreach (EventData item in data) {
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

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.EventData.SelectedItem is EventItem item) {
                this.TxtEventName.Text = item.Name;
            }
        }

        private void ClickRename(object sender, RoutedEventArgs e) {

        }
    }
}
