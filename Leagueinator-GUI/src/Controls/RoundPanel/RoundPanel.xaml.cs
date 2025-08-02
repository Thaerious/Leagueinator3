using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls.RoundPanel {
    /// <summary>
    /// Interaction logic for RoundPanel.xaml
    /// </summary>
    public partial class RoundPanel : UserControl {

        private int ActiveEventButton = -1;
        private int ActiveRoundButton = -1;
        private List<Button> EventButtons = [];
        private List<Button> RoundButtons = [];

        public RoundPanel() {
            NamedEvent.RegisterHandler(this, true);
            InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Round Panel Loaded");
            NamedEvent.ResumeEvents(this);
        }

        public Button AddEventButton(string text) {
            Button button = new() {
                Content = text,
                Style = (Style)this.FindResource("EventStyle"),
                DataContext = new EventButtonViewModel()
            };

            var menu = new ContextMenu();
            menu.AddMenuItem("Delete", this.HndDeleteEvent);
            menu.AddMenuItem("Settings", this.HndEventSettings);
            button.ContextMenu = menu;

            button.Click += this.HndClickEventButton;
            this.EventButtons.Add(button);
            return button;
        }

        public Button AddRoundButton(int index) {
            Button button = new() {
                Content = $"Round {index}",
                Style = (Style)this.FindResource("RoundStyle"),
                DataContext = new RoundButtonViewModel() { Index = index }
            };

            var menu = new ContextMenu();
            menu.AddMenuItem("Delete", this.HndDeleteRound);
            menu.AddMenuItem("Duplicate", this.HndDuplicateRound);
            button.ContextMenu = menu;
            button.Click += this.HndClickRoundButton;

            return button;
        }

        private void HndEventSettings(object sender, RoutedEventArgs e) {
            this.DispatchEvent(EventName.ShowEventSettings, []);
        }

        private void HndDeleteEvent(object sender, RoutedEventArgs e) {
            if (sender is not MenuItem menuItem) throw new NotSupportedException();
            if (menuItem.Parent is not ContextMenu contextMenu) throw new NotSupportedException();
            if (contextMenu.PlacementTarget is not Button button) throw new NotSupportedException();

            this.DispatchEvent(EventName.DeleteEvent, new() { 
                ["eventName"] = button.Content
            });
        }

        public void InsertAddRoundButton() {
            Button button = new() {
                Content = "Add Round",
                Style = (Style)this.FindResource("RoundStyle")
            };

            this.OuterPanel.Children.Add(button);
            button.Click += (s, e) => {
                this.DispatchEvent(EventName.AddRound, []);
            };
        }

        public void InsertAddEventButton() {
            Button button = new() {
                Content = "Add Event",
                Style = (Style)this.FindResource("EventStyle"),
            };

            this.OuterPanel.Children.Add(button);
            button.Click += (s, e) => {
                this.DispatchEvent(EventName.AddEvent, []);
            };
        }

        [NamedEventHandler(EventName.RoundChanged)]
        [NamedEventHandler(EventName.UpdateRoundCount)]
        [NamedEventHandler(EventName.RoundDeleted)]
        [NamedEventHandler(EventName.RoundAdded)]
        public void DoRoundUpdated(int roundCount, int roundIndex) {
            this.ActiveRoundButton = roundIndex;
            this.RefreshButtons(roundCount);
        }

        [NamedEventHandler(EventName.EventSelected)]
        internal void DoEventSelected(EventRecord eventRecord, int roundIndex, RoundRecordList roundRecords) {
            foreach (Button button in this.EventButtons) {
                if (button.DataContext is not EventButtonViewModel vme) throw new NotSupportedException();
                if ((string)button.Content == eventRecord.Name) {
                    vme.IsSelected = true;
                }
                else {
                    vme.IsSelected = false;
                }
            }

            this.ActiveRoundButton = roundIndex;
            this.RefreshButtons(eventRecord.RoundCount);
        }

        [NamedEventHandler(EventName.EventDeleted)]
        public void DoEventDeleted(string eventName) {
            var button = this.EventButtons.Where(b => (string)b.Content == eventName).First();
            this.EventButtons.Remove(button);
        }

        [NamedEventHandler(EventName.SetEventNames)]
        public void DoSetEventNames(List<string> eventNames, string selectedEvent, int roundCount, int roundIndex) {
            this.ActiveRoundButton = roundIndex;
            this.EventButtons.Clear();
            this.RefreshButtons(eventNames, selectedEvent, roundCount);
        }

        private void RefreshButtons(List<string> eventNames, string selectedEvent, int roundCount) {
            this.EventButtons.Clear();

            foreach (var name in eventNames) {
                Button button = this.AddEventButton(name);
                if (name == selectedEvent) {
                    if (button.DataContext is not EventButtonViewModel vm) throw new NotSupportedException();
                    vm.IsSelected = true;
                }
            }

            this.RefreshButtons(roundCount);
        }

        private void RefreshButtons(int roundButtonCount) {
            this.OuterPanel.Children.Clear();

            for (int i = 0; i < this.EventButtons.Count; i++) {
                Button eventButton = this.EventButtons[i];
                this.OuterPanel.Children.Add(eventButton);
                if (eventButton.DataContext is not EventButtonViewModel vme) throw new NotSupportedException();

                if (vme.IsSelected) {
                    this.RoundButtons.Clear();
                    for (int j = 0; j < roundButtonCount; j++) {
                        var roundButton = this.AddRoundButton(j + 1);
                        this.RoundButtons.Add(roundButton);
                        this.OuterPanel.Children.Add(roundButton);

                        if (this.ActiveRoundButton == j) {
                            if (roundButton.DataContext is not RoundButtonViewModel vm2) throw new NotSupportedException();
                            vm2.IsSelected = true;
                        }
                    }
                    this.InsertAddRoundButton();
                }                
            }

            this.InsertAddEventButton();
        }

        private void HndClickRoundButton(object sender, RoutedEventArgs e) {
            if (sender is not Button button) throw new NotSupportedException();
            this.DispatchEvent(EventName.SelectRound, new() {
                ["index"] = this.RoundButtons.IndexOf(button)
            });
        }

        private void HndClickEventButton(object sender, RoutedEventArgs e) {
            if (sender is not Button button) throw new NotSupportedException();
            this.DispatchEvent(EventName.SelectEvent, new() {
                ["index"] = this.EventButtons.IndexOf(button)
            });
        }

        private void HndDuplicateRound(object sender, RoutedEventArgs e) {
            this.DispatchEvent(EventName.CopyRound, []);
        }

        private void HndDeleteRound(object sender, RoutedEventArgs e) {
            this.DispatchEvent(EventName.DeleteRound, []);
        }
    }
}
