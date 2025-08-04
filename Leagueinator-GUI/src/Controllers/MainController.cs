using Leagueinator.GUI.Controllers.Algorithms;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Forms;
using Leagueinator.GUI.Forms.Event;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.ViewModel;
using Microsoft.Win32;
using System.IO;
using System.Windows;

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

        internal RoundData RoundData => this.EventData.Rounds[this.CurrentRoundIndex];

        private string FileName { get; set; } = "Leagueinator";

        private bool IsSaved { get; set; } = true;

        private MainWindow Window { get; set; }

        #endregion

        public MainController(MainWindow window) {
            this.Window = window;
            NamedEvent.RegisterHandler(this);
        }

        #region Dispatch Methods

        public void DispatchEventNames() {
            this.DispatchEvent(EventName.SetEventNames, new() {
                ["eventNames"]    = this.LeagueData.Select(e => e.EventName).ToList(),
                ["selectedEvent"] = EventData.EventName,
                ["eventRecord"]   = EventData.ToRecord(this.EventData),
                ["roundIndex"]    = this.CurrentRoundIndex,
                ["matchRecords"]  = MatchRecord.MatchRecordList(this.RoundData),
                ["records"]       = this.RoundData.Records(),
                ["roundCount"]    = this.EventData.Count(),
            });
        }

        public void DispatchRoundUpdated(EventName eventName) {
            this.DispatchEvent(eventName, new() {
                ["roundIndex"]   = this.CurrentRoundIndex,
                ["roundCount"]   = this.EventData.Count(),
                ["matchRecords"] = MatchRecord.MatchRecordList(this.RoundData),
                ["records"]      = this.RoundData.Records(),
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
                ["eventRecord"]  = EventData.ToRecord(this.EventData),
                ["roundIndex"]   = this.CurrentRoundIndex,
                ["matchRecords"] = MatchRecord.MatchRecordList(this.RoundData),
                ["records"]      = this.RoundData.Records(),
            });
        }

        [NamedEventHandler(EventName.AddEvent)]
        internal void DoAddEvent() {
            EventData eventData = this.LeagueData.AddEvent();
            eventData.DefaultEnds = this.EventData.DefaultEnds;
            eventData.DefaultLaneCount = this.EventData.DefaultLaneCount;
            eventData.DefaultMatchFormat = this.EventData.DefaultMatchFormat;
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
                eventData.DefaultLaneCount = int.Parse(form.TxtLanes.Text); ;
                eventData.DefaultMatchFormat = form.MatchFormat;

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
            this.EventData.ReplaceRound(this.CurrentRoundIndex, newRound);
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
                this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
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
            name = name.Trim();

            if (this.RoundData.HasPlayer(name)) {
                var record = this.RoundData.Records().Where(r => r.Name == name).First();
                this.RoundData.RemovePlayer(name);

                this.DispatchEvent(EventName.NameUpdated, new() {
                    ["lane"] = record.Lane,
                    ["teamIndex"] = record.Team,
                    ["position"] = record.Pos,
                    ["name"] = ""
                });
            }

            this.RoundData.Matches[lane].Teams[teamIndex].Set(position, name);
            this.DispatchEvent(EventName.NameUpdated, new() {
                ["lane"] = lane,
                ["teamIndex"] = teamIndex,
                ["position"] = position,
                ["name"] = name
            });
            this.DispatchSetTitle(this.FileName, false);
        }

        [NamedEventHandler(EventName.ChangeEnds)]
        internal void DoEnds(int lane, int ends) {
            if (lane < 0 || lane >= this.RoundData.Matches.Count) {
                throw new ArgumentOutOfRangeException($"Argument '{nameof(lane)}' value '{lane}' is not within [0 .. {this.RoundData.Matches.Count - 1}]");
            }

            this.RoundData.Matches[lane].Ends = ends;
            this.DispatchSetTitle(this.FileName, false);
            this.DispatchEvent(EventName.EndsUpdated, new() {
                ["lane"] = lane,
                ["ends"] = ends,
            });
        }

        [NamedEventHandler(EventName.ChangeTieBreaker)]
        internal void DoTieBreaker(int lane, int teamIndex) {
            this.RoundData.Matches[lane].TieBreaker = teamIndex;
            this.DispatchSetTitle(this.FileName, false);
            this.DispatchEvent(EventName.TieBreakerUpdated, new() {
                ["lane"] = lane,
                ["teamIndex"] = teamIndex,
            });
        }

        [NamedEventHandler(EventName.ChangeBowls)]
        internal void DoBowls(int lane, int teamIndex, int bowls) {
            this.RoundData.Matches[lane].Score[teamIndex] = bowls;
            this.DispatchSetTitle(this.FileName, false);

            this.DispatchEvent(EventName.BowlsUpdated, new() {
                ["lane"] = lane,
                ["bowls"] = (int[])this.RoundData.Matches[lane].Score.Clone()
            });
        }

        [NamedEventHandler(EventName.ChangeMatchFormat)]
        internal void DoMatchFormat(int lane, MatchFormat format) {
            this.RoundData.Matches[lane].MatchFormat = format;
            this.DispatchRoundUpdated(EventName.RoundChanged);
            this.DispatchSetTitle(this.FileName, false);
        }

        [NamedEventHandler(EventName.RemoveMatch)]
        internal void DoRemoveMatch(int lane) {
            throw new NotImplementedException("Discontinued");
        }

        [NamedEventHandler(EventName.SwapTeams)]
        internal void DoSwap(int fromLane, int toLane, int fromIndex, int toIndex) {
            List<string> namesFrom = [.. this.RoundData.Matches[fromLane].Teams[fromIndex].Names];
            List<string> namesTo = [.. this.RoundData.Matches[toLane].Teams[toIndex].Names];

            this.RoundData.Matches[fromLane].Teams[fromIndex].CopyFrom(namesTo);
            this.RoundData.Matches[toLane].Teams[toIndex].CopyFrom(namesFrom);
            this.DispatchRoundUpdated(EventName.RoundChanged);
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
            if (this.EventData.Rounds.Count == 0) {
                this.EventData.AddRound();
                this.CurrentRoundIndex = 0;
            }
            else if (this.CurrentRoundIndex >= this.EventData.Rounds.Count) {
                this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
            }
        }

        private bool UpdateName(string name, int lane, int teamIndex, int position) {
            if (name == string.Empty && this.RoundData.Matches[lane].Teams[teamIndex].Names[position] == string.Empty) {
                // If the eventName is empty and the player is already empty, do nothing.
                return false;
            }

            if (

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

            this.RoundData.Matches[lane].SetPlayer(name, teamIndex, position);
            return true;
        }

        /// <summary>
        /// Synchronizes the provided <paramref eventName="roundRecords"/> with the current event settings.
        /// <para>
        /// - Removes empty lanes if there are more lanes than specified in <see cref="EventData.DefaultLaneCount"/>.
        /// - Adds new empty lanes if there are fewer lanes than <see cref="EventData.DefaultLaneCount"/>.
        /// - Ensures all <see cref="MatchData.Lane"/> values are set to their correct indices.
        /// - Sets the ends for unplayed matches to <see cref="EventData.DefaultEnds"/>.
        /// - Updates the match format for empty matches to match <see cref="EventData.DefaultMatchFormat"/>.
        /// </para>
        /// </summary>
        /// <param eventName="roundRecords">The round LeagueData to update, representing a collection of matches for a round.</param>        
        private static void SyncRoundData(RoundData roundData, EventData eventData) {
            // RoundChanged LeagueData by removing empty lanes until the number of lanes matches the event LeagueData's lane count.
            for (int i = roundData.Matches.Count - 1; i >= 0; i--) {
                if (roundData.Matches.Count <= eventData.DefaultLaneCount) break;
                if (roundData.Matches[i].CountPlayers() != 0) continue;
                roundData.RemoveAt(i);
            }

            // If the number of lanes is less than the event LeagueData's lane count, add new empty lanes.
            while (roundData.Count < eventData.DefaultLaneCount) {
                roundData.Add(new MatchData() {
                    MatchFormat = eventData.DefaultMatchFormat,
                    Lane = roundData.Count + 1
                });
            }

            // Ensure all match LeagueData lanes are set correctly.
            for (int i = 0; i < roundData.Count; i++) {
                RoundData.Matches[i].Lane = i;
            }

            // Change unplayed match ends to the event's default ends value.
            foreach (MatchData matchData in roundData) {
                if (matchData.Score.Sum() != 0) continue; // If the match has a score, it is played.
                matchData.Ends = eventData.DefaultEnds;
            }

            // Ensure all empty match LeagueData has the same match format as the event.
            foreach (MatchData matchData in roundData) {
                if (matchData.CountPlayers() != 0) continue;
                matchData.MatchFormat = eventData.DefaultMatchFormat;
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
            sb += $"Number of Lanes: {this.EventData.DefaultLaneCount}\n";
            sb += $"Default Ends: {this.EventData.DefaultEnds}\n";
            sb += $"Match Format: {this.EventData.DefaultMatchFormat}\n";
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

            this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
            this.DispatchRoundUpdated(EventName.RoundAdded);
            this.DispatchSetTitle(this.FileName, false);
        }
        #endregion
    }
}
