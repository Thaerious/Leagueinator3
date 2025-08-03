using Leagueinator.GUI.Controllers.Algorithms;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Forms;
using Leagueinator.GUI.Forms.Event;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;

namespace Leagueinator.GUI.Controllers {

    /// <summary>
    /// This glues the MainWindow window to the model and handles events from the MainWindow window.
    /// </summary>
    public partial class MainController {

        #region Properties
        internal LeagueData LeagueData { get; private set; } = [];

        private EventData? _eventData = default;
        internal EventData EventData {
            get {
                if (_eventData == null) throw new NullReferenceException("EventData not set.");
                return _eventData;
            }
            set {
                this._eventData = value;
                var module = EventTypeMeta.GetModule(this._eventData.EventType);
                this.LoadModule(module);
            }
        }

        private int CurrentRoundIndex { get; set; } = 0;

        internal RoundData RoundData => this.EventData.GetRound(this.CurrentRoundIndex);

        private string FileName { get; set; } = "Leagueinator";

        private bool IsSaved { get; set; } = true;

        private MainWindow Window { get; set; }

        #endregion

        public MainController(MainWindow window) {
            this.Window = window;
            this.NewLeague();
        }

        #region Dispatch Methods

        public void DispatchEventNames() {
            this.DispatchEvent(EventName.SetEventNames, new() {
                ["eventNames"]    = this.LeagueData.Select(e => e.EventName).ToList(),
                ["selectedEvent"] = EventData.EventName,
                ["eventRecord"]   = EventData.ToRecord(this.EventData),
                ["roundIndex"]    = this.CurrentRoundIndex,
                ["roundRecords"]  = new RoundRecordList(this.EventData, this.RoundData),
                ["roundCount"]    = this.EventData.Count(),
            });
        }

        public void DispatchRoundUpdated(EventName eventName) {
            this.DispatchEvent(eventName, new() {
                ["roundIndex"]   = this.CurrentRoundIndex,
                ["roundCount"]   = this.EventData.Count(),
                ["roundRecords"] = new RoundRecordList(this.EventData, this.RoundData),
            });
        }

        public void DispatchSetTitle(string title, bool saved) {
            this.IsSaved = saved;
            this.DispatchEvent(EventName.SetTitle, new() {
                ["title"] = title,
                ["saved"] = saved
            });
        }

        #endregion

        #region Event Handlers
        [NamedEventHandler(EventName.RenameEvent)]
        internal void DoRenameEvent(string from, string to) {
            if (this.LeagueData.Select(e => e.EventName).Where(name => name == to).Any()) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["message"] = $"Event name '{to}' already exits."
                });
                return;
            }

            if (!this.LeagueData.Select(e => e.EventName).Where(name => name == from).Any()) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["message"] = $"Event name '{from}' does not exist."
                });
                return;
            }


            this.LeagueData
                .Where(e => e.EventName == from)
                .First()
                .EventName = to;
        }

        [NamedEventHandler(EventName.SelectEvent)]
        internal void DoSelectEvent(int index) {
            this.EventData = this.LeagueData[index];
            this.CurrentRoundIndex = this.EventData.Count() - 1;

            this.DispatchEvent(EventName.EventSelected, new() {
                ["eventRecord"] = EventData.ToRecord(this.EventData),
                ["roundIndex"] = this.CurrentRoundIndex,
                ["roundRecords"] = new RoundRecordList(this.EventData, this.RoundData),
            });
        }

        [NamedEventHandler(EventName.AddEvent)]
        internal void DoAddEvent() {
            EventData eventData = this.LeagueData.AddEvent();
            eventData.DefaultEnds = this.EventData.DefaultEnds;
            eventData.LaneCount = this.EventData.LaneCount;
            eventData.MatchFormat = this.EventData.MatchFormat;
            eventData.EventType = this.EventData.EventType;
            eventData.AddRound();

            this.EventData = eventData;
            this.CurrentRoundIndex = this.EventData.Count() - 1;

            this.DispatchEventNames();
        }

        [NamedEventHandler(EventName.DeleteEvent)]
        internal void DoDeleteEvent(string eventName) {
            if (this.LeagueData.Count < 2) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["message"] = "Can not delete last event."
                });
                return;
            }

            EventData eventData = this.LeagueData.Where(e => e.EventName == eventName).First();
            this.LeagueData.Remove(eventData);
            this.EventData = this.LeagueData.Last();
            this.CurrentRoundIndex = this.EventData.Count() - 1;
            this.DispatchEventNames();
        }

        [NamedEventHandler(EventName.ShowEventSettings)]
        internal void DoShowEventSettings(string eventName) {
            var form = new EventSettingsForm() {
                Owner = this.Window,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            List<EventRecord> records = [.. this.LeagueData.Select(data => EventData.ToRecord(data))];
            form.PauseEvents();
            NamedEvent.RegisterHandler(form);

            EventData eventData = this.LeagueData.Where(e => e.EventName == eventName).First();

            if (form.ShowDialog(eventData) ?? false) {
                this.DispatchEventNames();
                NamedEvent.RemoveHandler(form);

                eventData.EventName = form.TxtName.Text;
                eventData.DefaultEnds = int.Parse(form.TxtEnds.Text);
                eventData.LaneCount = int.Parse(form.TxtLanes.Text); ;
                eventData.MatchFormat = form.MatchFormat;

                //SyncRoundData(this.RoundData, this.EventData); // ??? DO I STILL NEED THIS
                this.DispatchEventNames();
            }
        }

        #endregion

        #region League Handlers

        [NamedEventHandler(EventName.LoadLeague)]
        internal void DoLoad() {
            this.Load();
            this.DispatchEventNames();
            this.DispatchSetTitle(this.FileName, true);
        }

        [NamedEventHandler(EventName.SaveLeague)]
        internal void DoSave() {
            if (this.FileName != "Leagueinator") {
                this.Save(this.FileName);
            }
            else {
                this.SaveAs();
            }
            this.DispatchSetTitle(this.FileName, true);
        }

        [NamedEventHandler(EventName.SaveLeagueAs)]
        internal void DoSaveAs() {
            this.SaveAs();
            this.DispatchSetTitle(this.FileName, true);
        }

        [NamedEventHandler(EventName.NewLeague)]
        internal void DoNew() {
            this.NewLeague();
            this.DispatchEventNames();
            this.DispatchSetTitle("Leagueinator", true);
            this.FileName = "Leagueinator";
        }
        #endregion

        #region Round Handlers
        [NamedEventHandler(EventName.AssignLanes)]
        internal void DoAssignLanes() {
            AssignLanes assignLanes = new(this.EventData, this.RoundData);
            RoundData newRound = assignLanes.Run();
            this.EventData.SetRound(this.CurrentRoundIndex, newRound);
            this.DispatchRoundUpdated(EventName.RoundChanged);
            this.DispatchSetTitle(this.FileName, false);
        }

        [NamedEventHandler(EventName.AddRound)]
        internal void DoAddRound() {
            RoundData newRound = this.EventData.AddRound();
            this.AddRound(newRound);
        }

        [NamedEventHandler(EventName.DeleteRound)]
        internal void DoDeleteRound(int? roundIndex = null) {
            int index = roundIndex ?? this.CurrentRoundIndex;
            this.RemoveRound(index);
            this.DispatchRoundUpdated(EventName.RoundDeleted);
            this.DispatchSetTitle(this.FileName, false);
        }

        [NamedEventHandler(EventName.SelectRound)]
        internal void DoSelectRound(int index = -1) {

            if (index == -1) {
                this.CurrentRoundIndex = this.EventData.CountRounds() - 1;
            }
            else {
                this.CurrentRoundIndex = index;
            }
            this.DispatchRoundUpdated(EventName.RoundChanged);
        }

        [NamedEventHandler(EventName.CopyRound)]
        internal void DoCopyRound() {
            RoundData newRound = this.RoundData.Copy();
            this.EventData.AddRound(newRound);
            this.AddRound(newRound);
        }
        #endregion

        #region Report Handlers

        [NamedEventHandler(EventName.DisplayText)]
        internal void DoDisplayText(string text) {
            TextViewer tv = new();
            tv.Append(text);
            tv.Show();
        }

        [NamedEventHandler(EventName.ShowData)]
        internal void DoShow() {
            TextViewer tv = new TextViewer();
            tv.Append(this.GetShow());
            tv.Show();
        }
        #endregion

        # region Match, Team, Player Handlers  
        [NamedEventHandler(EventName.ChangePlayerName)]
        internal void DoPlayerName(string name, int lane, int teamIndex, int position) {

            if (this.UpdateName(name, lane, teamIndex, position)) {
                this.DispatchSetTitle(this.FileName, false);
                this.DispatchEvent(EventName.NameUpdated, new() {
                    ["lane"] = lane,
                    ["teamIndex"] = teamIndex,
                    ["position"] = position,
                    ["name"] = name
                });
            }
        }

        [NamedEventHandler(EventName.ChangeEnds)]
        internal void DoEnds(int lane, int ends) {
            if (lane < 0 || lane >= this.RoundData.Count) {
                throw new ArgumentOutOfRangeException($"Argument '{nameof(lane)}' value '{lane}' is not within [0 .. {this.RoundData.Count - 1}]");
            }

            this.RoundData[lane].Ends = ends;
            this.DispatchSetTitle(this.FileName, false);
            this.DispatchEvent(EventName.EndsUpdated, new() {
                ["lane"] = lane,
                ["ends"] = ends,
            });
        }

        [NamedEventHandler(EventName.ChangeTieBreaker)]
        internal void DoTieBreaker(int lane, int teamIndex) {
            this.RoundData[lane].TieBreaker = teamIndex;
            this.DispatchSetTitle(this.FileName, false);
            this.DispatchEvent(EventName.TieBreakerUpdated, new() {
                ["lane"] = lane,
                ["teamIndex"] = teamIndex,
            });
        }

        [NamedEventHandler(EventName.ChangeBowls)]
        internal void DoBowls(int lane, int teamIndex, int bowls) {
            this.RoundData[lane].Score[teamIndex] = bowls;
            this.DispatchSetTitle(this.FileName, false);

            this.DispatchEvent(EventName.BowlsUpdated, new() {
                ["lane"] = lane,
                ["bowls"] = (int[])this.RoundData[lane].Score.Clone()
            });
        }

        [NamedEventHandler(EventName.ChangeMatchFormat)]
        internal void DoMatchFormat(int lane, MatchFormat format) {
            this.RoundData[lane].MatchFormat = format;
            this.DispatchRoundUpdated(EventName.RoundChanged);
            this.DispatchSetTitle(this.FileName, false);
        }

        [NamedEventHandler(EventName.RemoveMatch)]
        internal void DoRemoveMatch(int lane) {
            this.RoundData.RemoveAt(lane);
            this.DispatchEvent(EventName.MatchRemoved, new() {
                ["lane"] = lane
            });
            this.DispatchSetTitle(this.FileName, false);
        }
        #endregion

        #region Private Methods
        private void NewLeague() {
            if (!this.IsSaved) {
                ConfirmationDialog confDialog = new() {
                    Owner = this.Window,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Text = "League not saved. Do you still want to create a new one?"
                };

                if (confDialog.ShowDialog() == false) return;
            }

            this.LeagueData = [];
            this.LeagueData.AddEvent();
            this.EventData = this.LeagueData[0];
            RoundData newRound = this.EventData.AddRound();
            this.CurrentRoundIndex = 0;
        }

        private void Load() {
            if (!this.IsSaved) {
                ConfirmationDialog confDialog = new() {
                    Owner = this.Window,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Text = "League not saved. Do you still want to load?"
                };

                if (confDialog.ShowDialog() == false) return;
            }

            OpenFileDialog dialog = new OpenFileDialog {
                Filter = "League Files (*.league)|*.league"
            };

            if (dialog.ShowDialog() == true) {
                string contents = File.ReadAllText(dialog.FileName);
                this.LeagueData = LeagueData.FromString(contents);
                this.EventData = this.LeagueData.Last();
                this.CurrentRoundIndex = this.EventData.Count() - 1;
                this.FileName = dialog.FileName;
                this.IsSaved = true;
            }
        }

        private void SaveAs() {
            SaveFileDialog dialog = new() {
                Filter = "League Files (*.league)|*.league"
            };

            if (dialog.ShowDialog() == true) {
                this.Save(dialog.FileName);
            }
        }

        private void Save(string filename) {
            // Serialize the EventData and RoundCollection to JSON and write to the file.
            StreamWriter writer = new(filename);
            //string json = JsonSerializer.Serialize(this.LeagueData, this.GetOptions());
            writer.Write(this.LeagueData.ToString());
            writer.Close();

            this.FileName = filename;
            this.IsSaved = true;
        }

        private void RemoveRound(int index) {
            this.EventData.RemoveRound(index);
            //if (this.CurrentRoundIndex > index) {
            //    this.CurrentRoundIndex--;
            //}

            if (this.EventData.CountRounds() == 0) {
                this.EventData.AddRound();
                this.CurrentRoundIndex = 0;
            }
            else if (this.CurrentRoundIndex >= this.EventData.CountRounds()) {
                this.CurrentRoundIndex = this.EventData.CountRounds() - 1;
            }
        }

        private bool UpdateName(string name, int lane, int teamIndex, int position) {
            if (name == string.Empty && this.RoundData[lane].Teams[teamIndex][position] == string.Empty) {
                // If the eventName is empty and the player is already empty, do nothing.
                return false;
            }

            PlayerLocation poll = this.RoundData.PollPlayer(name);
            if (poll == new PlayerLocation(lane, teamIndex, position)) {
                return false;
            }

            // If the player already exists in the round LeagueData, remove them from their current position
            // and remove their eventName from the previous match card.
            if (this.RoundData.HasPlayer(name)) {
                var existing = this.RoundData.PollPlayer(name);
                this.RoundData.RemovePlayer(name);

                this.DispatchEvent(EventName.NameUpdated, new() {
                    ["lane"] = existing.Lane,
                    ["teamIndex"] = existing.TeamIndex,
                    ["position"] = existing.Position,
                    ["name"] = ""
                });
            }

            this.RoundData.SetPlayer(name, lane, teamIndex, position);
            return true;
        }

        /// <summary>
        /// Synchronizes the provided <paramref eventName="roundRecords"/> with the current event settings.
        /// <para>
        /// - Removes empty lanes if there are more lanes than specified in <see cref="EventData.LaneCount"/>.
        /// - Adds new empty lanes if there are fewer lanes than <see cref="EventData.LaneCount"/>.
        /// - Ensures all <see cref="MatchData.Lane"/> values are set to their correct indices.
        /// - Sets the ends for unplayed matches to <see cref="EventData.DefaultEnds"/>.
        /// - Updates the match format for empty matches to match <see cref="EventData.MatchFormat"/>.
        /// </para>
        /// </summary>
        /// <param eventName="roundRecords">The round LeagueData to update, representing a collection of matches for a round.</param>        
        private static void SyncRoundData(RoundData roundData, EventData eventData) {
            // RoundChanged LeagueData by removing empty lanes until the number of lanes matches the event LeagueData's lane count.
            for (int i = roundData.Count - 1; i >= 0; i--) {
                if (roundData.Count <= eventData.LaneCount) break;
                if (roundData[i].CountPlayers() != 0) continue;
                roundData.RemoveAt(i);
            }

            // If the number of lanes is less than the event LeagueData's lane count, add new empty lanes.
            while (roundData.Count < eventData.LaneCount) {
                roundData.Add(new MatchData() {
                    MatchFormat = eventData.MatchFormat,
                    Lane = roundData.Count + 1
                });
            }

            // Ensure all match LeagueData lanes are set correctly.
            for (int i = 0; i < roundData.Count; i++) {
                roundData[i].Lane = i;
            }

            // Change unplayed match ends to the event's default ends value.
            foreach (MatchData matchData in roundData) {
                if (matchData.Score.Sum() != 0) continue; // If the match has a score, it is played.
                matchData.Ends = eventData.DefaultEnds;
            }

            // Ensure all empty match LeagueData has the same match format as the event.
            foreach (MatchData matchData in roundData) {
                if (matchData.CountPlayers() != 0) continue;
                matchData.MatchFormat = eventData.MatchFormat;
            }
        }

        private string GetShow() {
            string sb = string.Empty;
            sb += "Controller Data\n";
            sb += $"File Name: {this.FileName}\n";
            sb += $"Is Saved: {this.IsSaved}\n";
            sb += $"Current Round Index: {this.CurrentRoundIndex}/{this.EventData.Count()}\n";
            sb += $"No. of Events: {this.LeagueData.Count}\n";
            sb += "\nEvent Data\n";
            sb += $"Event Name: {this.EventData.EventName}\n";
            sb += $"Number of Lanes: {this.EventData.LaneCount}\n";
            sb += $"Default Ends: {this.EventData.DefaultEnds}\n";
            sb += $"Match Format: {this.EventData.MatchFormat}\n";
            sb += $"Event Type: {this.EventData.EventType}\n";
            sb += "";
            sb += $"Current Round: {this.CurrentRoundIndex}\n";

            foreach (RoundData round in this.EventData) {
                sb += $"Round {this.EventData.IndexOf(round) + 1}:\n";
                sb += round;
                sb += "\n";
            }
            return sb;
        }

        #endregion

        #region Public Methods
        public void AddRound(RoundData newRound) {
            if (!this.EventData.Contains(newRound)) {
                this.EventData.AddRound(newRound);
            }

            this.CurrentRoundIndex = this.EventData.CountRounds() - 1;
            this.DispatchRoundUpdated(EventName.RoundAdded);
            this.DispatchSetTitle(this.FileName, false);
        }
        #endregion
    }
}
