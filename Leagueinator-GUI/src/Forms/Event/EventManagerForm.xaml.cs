﻿using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using EventRecord = Leagueinator.GUI.Model.EventRecord;

namespace Leagueinator.GUI.Forms.Event {

    public record MatchFormatRecord(MatchFormat MatchFormat, string DisplayName) { }
    public record EventTypeRecord(EventType EventType, string DisplayName) { }

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
                    new(MatchFormat.VS1, "1 vs 1"),
                    new(MatchFormat.VS2, "2 vs 2"),
                    new(MatchFormat.VS3, "3 vs 3"),
                    new(MatchFormat.VS4, "4 vs 4"),
                    new(MatchFormat.A4321, "4321"),
                };
                this.ListMatchFormat.DisplayMemberPath = "DisplayName";

                this.ListEventType.ItemsSource = new List<EventTypeRecord> {
                    new(EventType.RankedLadder, "Ranked Ladder"),
                    //new(EventType.Motley, "Motley"), TODO ENABLE
                    //new(EventType.RoundRobin, "Round Robin"), TODO ENABLE
                };
                this.ListEventType.DisplayMemberPath = "DisplayName";
            };
        }

        public void ShowDialog(MainController mainController, List<EventRecord> eventRecords, EventRecord selected) {
            foreach (var record in eventRecords) this.EventRecords.Add(record);
            this.DataContext = this;
            this.DoEventChanged(selected);
            this.ShowDialog();
        }

        #region Button Handlers

        private void HndNew(object sender, EventArgs e) {
            MainWindow.NamedEventDisp.Dispatch(EventName.AddEvent);
        }

        private void HndDelete(object sender, EventArgs e) {
            MainWindow.NamedEventDisp.Dispatch(EventName.DeleteEvent, new() {
                ["eventUID"] = (this.EventData.SelectedItem as EventRecord)!.UID,
            });
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


        [NamedEventHandler(EventName.EventChanged)]
        internal void DoEventChanged(EventRecord eventRecord) {
            MainWindow.NamedEventDisp.PauseEvents();
            this.EventData.SelectedItem = eventRecord;
            this.TxtName.Text = eventRecord.Name;
            this.TxtEnds.Text = eventRecord.DefaultEnds.ToString();
            this.TxtLanes.Text = eventRecord.LaneCount.ToString();
            MainWindow.NamedEventDisp.ResumeEvents();
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

        #endregion

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.EventData.SelectedItem is not EventRecord record) return;
            this.ButDelete.IsEnabled = true;
            MainWindow.NamedEventDisp.Dispatch(EventName.SelectEvent, new() {
                ["uid"] = record.UID,
            });
        }

        private void TxtChanged(object sender, TextChangedEventArgs args) {
            this.InvokeChangeEventArg();
        }

        private void EventTypeChanged(object sender, SelectionChangedEventArgs e) {
            if (this.IsLoaded == false) return;

            EventTypeRecord etf = (EventTypeRecord)this.ListEventType.SelectedItem;

            MainWindow.NamedEventDisp.Dispatch(EventName.ChangeEventType, new() {
                ["eventType"] = etf.EventType
            });
        }

        private void MatchFormatChanged(object sender, SelectionChangedEventArgs e) {
            this.InvokeChangeEventArg();
        }

        private void InvokeChangeEventArg() {
            if (this.IsLoaded == false) return;

            MatchFormatRecord mfr = (MatchFormatRecord)this.ListMatchFormat.SelectedItem;

            MainWindow.NamedEventDisp.Dispatch(EventName.ChangeEventArg, new() {
                ["name"] = this.TxtName.Text,
                ["laneCount"] = int.Parse(this.TxtLanes.Text),
                ["ends"] = int.Parse(this.TxtEnds.Text),
                ["matchFormat"] = mfr.MatchFormat
            });
        }
    }
}
