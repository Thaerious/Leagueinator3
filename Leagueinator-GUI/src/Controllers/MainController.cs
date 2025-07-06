using Leagueinator.GUI.Controllers.Algorithms;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Forms;
using Leagueinator.GUI.Forms.Event;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;
using Leagueinator.GUI.Utility;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Leagueinator.GUI.Controllers {

    /// <summary>
    /// This glues the MainWindow window to the model and handles events from the MainWindow window.
    /// </summary>
    public class MainController {
        public delegate void SetFilename(object sender, string filename, bool saved);
        public delegate void SetRoundCount(object sender, int count);
        public delegate void UpdateRound(object sender, RoundEventData roundData);

        public event SetFilename OnSetTitle = delegate { };
        public event SetRoundCount OnSetRoundCount = delegate { };
        public event UpdateRound OnUpdateRound = delegate { };

        public NamedEventReceiver NamedEventRcv { get; private set; }

        internal LeagueData LeagueData { get; private set; } = [];

        internal RoundData RoundData {
            get => this.EventData.Rounds[this.CurrentRoundIndex];
        }

        private int CurrentRoundIndex { get; set; } = 0;


        private EventData? _eventData = default;
        internal EventData EventData {
            get {
                if (_eventData == null) throw new NullReferenceException("EventData not set.");
                return _eventData;
            }
            set => this._eventData = value;
        }

        private string FileName { get; set; } = "Leagueinator";

        private bool IsSaved { get; set; } = true;

        private MainWindow Window { get; set; }

        public MainController(MainWindow window) {
            this.NamedEventRcv = new(this);
            this.Window = window;
            this.NewLeague();
        }

        public void InvokeRoundEvent(string action) {
            this.OnUpdateRound.Invoke(this, new(action, this.CurrentRoundIndex, this.RoundData));
        }

        public void InvokeRoundEvent(string action, int index) {
            this.OnUpdateRound.Invoke(this, new(action, index, this.EventData.Rounds[index]));
        }

        public void InvokeRoundEvent(string action, int index, RoundData? roundData) {
            this.OnUpdateRound.Invoke(this, new(action, index, roundData));
        }

        public void InvokeSetTitle(string filename, bool saved) {
            this.IsSaved = saved;
            this.OnSetTitle.Invoke(this, filename, saved);
        }

        internal void DoEventManager() {
            Logger.Log("MainController.DoEventManager");
            var form = new EventManagerForm();
            form.ShowDialog(this, this.LeagueData);
        }

        internal void DoLoad() {
            Logger.Log("MainController.DoLoad");
            this.Load();
            this.OnSetRoundCount.Invoke(this, this.EventData.Rounds.Count);
            this.InvokeRoundEvent("Update");
            this.InvokeSetTitle(this.FileName, true);
        }

        internal void DoRenameEvent(string name, int uid) {
            Logger.Log("MainController.DoRenameEvent");
            foreach (EventData eventData in this.LeagueData) {
                if (eventData.UID == uid) {
                    eventData.EventName = name;
                    return;
                }
            }
        }

        internal void DoCreateEvent() {
            this.LeagueData.Add(new EventData() { UID = this.LeagueData.GetNextUID() });

        }

        internal void DoSave() {
            Logger.Log("MainController.DoSave");
            if (this.FileName != "Leagueinator") {
                this.Save(this.FileName);
            }
            else {
                this.SaveAs();
            }
            this.InvokeSetTitle(this.FileName, true);
        }

        internal void DoSaveAs() {
            Logger.Log("MainController.DoSaveAs");
            this.SaveAs();
            this.InvokeSetTitle(this.FileName, true);
        }

        internal void DoNew() {
            Logger.Log("MainController.DoNew");
            this.NewLeague();

            this.OnSetRoundCount.Invoke(this, EventData.Rounds.Count);
            this.InvokeRoundEvent("Update");
            this.InvokeSetTitle("Leagueinator", true);
            this.FileName = "Leagueinator";
        }

        internal void DoPrintTeams() {
            Logger.Log("MainController.DoPrintTeams");
            PrintWindow pw = new(this.EventData.Rounds);
            pw.Show();
        }

        internal void DoAssignLanes() {
            Logger.Log("MainController.DoAssignLanes");
            AssignLanes assignLanes = new(this.EventData, EventData.Rounds, this.RoundData); // TODO can just pass this.EventData
            RoundData newRound = assignLanes.DoAssignment();
            this.EventData.Rounds.Add(newRound);
            this.InvokeSetTitle(this.FileName, false);
            this.InvokeRoundEvent("Update");
        }

        internal void DoGenerateRound() {
            Logger.Log("MainController.DoGenerateRound");
            var newRound = this.GenerateRound();
            AssignLanes assignLanes = new(this.EventData, this.EventData.Rounds, newRound); // TODO can just pass this.EventData
            newRound = assignLanes.DoAssignment();
            this.EventData.Rounds.Add(newRound);
            this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
            this.InvokeRoundEvent("AddRound", this.CurrentRoundIndex, newRound);
            this.InvokeSetTitle(this.FileName, false);
            this.InvokeRoundEvent("Update");
        }

        internal void DoAddRound() {
            Logger.Log("MainController.DoAddRound");
            RoundData newRound = new(this.EventData);
            this.EventData.Rounds.Add(newRound);
            var createdIndex = this.EventData.Rounds.Count - 1;
            this.InvokeRoundEvent("AddRound", createdIndex, newRound);
            this.InvokeSetTitle(this.FileName, false);
        }

        internal void DoRemoveRound() {
            Logger.Log("MainController.DoRemoveRound");
            var previousIndex = this.CurrentRoundIndex;
            this.RemoveRound(this.CurrentRoundIndex);
            this.InvokeRoundEvent("Update");
            this.InvokeRoundEvent("RemoveRound", previousIndex, null);
            this.InvokeSetTitle(this.FileName, false);
        }

        internal void DoSelectRound(int index = -1) {
            Logger.Log("MainController.DoSelectRound");

            if (index == -1) {
                this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
            }
            else {
                this.CurrentRoundIndex = index;
            }
            this.InvokeRoundEvent("Update");
        }

        internal void DoCopyRound() {
            Logger.Log("MainController.DoCopyRound");
            this.EventData.Rounds.Add(this.RoundData.Copy());
            var copiedIndex = this.EventData.Rounds.Count - 1;
            this.InvokeRoundEvent("AddRound", copiedIndex);
            this.InvokeSetTitle(this.FileName, false);
        }

        internal void DoAssignPlayersRandomly() {
            Logger.Log("MainController.DoAssignPlayersRandomly");
            this.RoundData.AssignPlayersRandomly();
            this.InvokeRoundEvent("Update");
            this.InvokeSetTitle(this.FileName, false);
        }

        internal void DoRoundResults() {
            Logger.Log("MainController.DoRoundResults");
            RoundResults rr = new(this.RoundData);
            TableViewer tv = new TableViewer();

            foreach (SingleResult result in rr.Results) {
                tv.Append(result.ToString());
            }

            tv.Show();
        }

        internal void DoEventResults(object? sender, NamedEventArgs e) {
            Logger.Log("MainController.DoEventResults");
            EventResults er = new(this.EventData.Rounds);
            TableViewer tv = new TableViewer();
            foreach (TeamResult result in er.ResultsByTeam) {
                tv.Append(result.ToString());
            }
            tv.Show();
        }

        internal void DoShow() {
            Logger.Log("MainController.DoShow");
            TableViewer tv = new TableViewer();
            tv.Append("Event Data:");
            tv.Append($"File Name: {this.FileName}");
            tv.Append($"Is Saved: {this.IsSaved}");
            tv.Append($"Number of Lanes: {this.EventData.LaneCount}");
            tv.Append($"Default Ends: {this.EventData.DefaultEnds}");
            tv.Append($"Match Format: {this.EventData.MatchFormat}");
            tv.Append($"Event Type: {this.EventData.EventType}");
            tv.Append("");
            tv.Append($"Current Round: {this.CurrentRoundIndex}");
            foreach (RoundData round in this.EventData.Rounds) {
                tv.Append($"Round {this.EventData.Rounds.IndexOf(round) + 1}:");
                tv.Append(round);
            }
            tv.Show();
        }

        internal void DoPlayerName(string name, int lane, int teamIndex, int position) {
            Logger.Log("MainController.DoPlayerName");

            if (this.UpdateName(name, lane, teamIndex, position)) {
                this.InvokeRoundEvent("Update");
                this.InvokeSetTitle(this.FileName, false);
            }
        }

        internal void DoEnds(int lane, int ends) {
            Logger.Log("MainController.DoEnds");

            this.RoundData[lane].Ends = ends;
            this.InvokeRoundEvent("Update");
            this.InvokeSetTitle(this.FileName, false);
        }

        internal void DoTieBreaker(int lane, int tieBreaker) {
            Logger.Log("MainController.DoTieBreaker");
            this.RoundData[lane].TieBreaker = tieBreaker;
            this.InvokeRoundEvent("Update");
            this.InvokeSetTitle(this.FileName, false);
        }

        internal void DoBowls(int lane, int teamIndex, int bowls) {
            Logger.Log("MainController.DoBowls");
            this.RoundData[lane].Score[teamIndex] = bowls;
            this.InvokeRoundEvent("Update");
            this.InvokeSetTitle(this.FileName, false);
        }

        internal void DoMatchFormat(int lane, MatchFormat format) {
            Logger.Log("MainController.DoMatchFormat");
            this.RoundData[lane].MatchFormat = format;
            this.InvokeRoundEvent("Update");
            this.InvokeSetTitle(this.FileName, false);
        }

        internal void DoRemoveMatch(int lane) {
            Logger.Log("MainController.DoRemoveMatch");
            this.RoundData.RemoveAt(lane);
            this.InvokeRoundEvent("RemoveMatch", lane);
            this.InvokeSetTitle(this.FileName, false);
        }

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
            this.LeagueData.Add(new EventData() { UID = this.LeagueData.GetNextUID() });
            this.EventData = this.LeagueData[0];
            RoundData newRound = new(this.EventData);
            this.EventData.Rounds.Add(newRound);
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
                LeagueData leagueData = JsonSerializer.Deserialize<LeagueData>(contents, this.GetOptions())
                    ?? throw new InvalidDataException("Failed to deserialize the file contents.");

                this.EventData = leagueData.Last();
                this.CurrentRoundIndex = 0;
                this.FileName = dialog.FileName;
                this.IsSaved = true;
            }
        }

        private JsonSerializerOptions GetOptions() {
            return new JsonSerializerOptions { WriteIndented = true };
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
            string json = JsonSerializer.Serialize(this.LeagueData, this.GetOptions());
            writer.WriteLine(json);
            writer.Close();

            this.FileName = filename;
            this.IsSaved = true;
        }

        private void RemoveRound(int index) {
            this.EventData.Rounds.RemoveAt(index);

            if (this.EventData.Rounds.Count == 0) {
                this.EventData.Rounds.Add(new RoundData(this.EventData));
                this.CurrentRoundIndex = 0;
            }
            else if (this.CurrentRoundIndex >= this.EventData.Rounds.Count) {
                this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
            }
            Debug.WriteLine($"Removed round at index {index}. Current round index is now {this.CurrentRoundIndex}.");
        }

        private bool UpdateName(string name, int lane, int team, int pos) {
            if (name == string.Empty && this.RoundData[lane].Teams[team][pos] == string.Empty) {
                // If the name is empty and the player is already empty, do nothing.
                return false;
            }

            var poll = this.RoundData.PollPlayer(name);
            if (poll == (lane, team, pos)) {
                return false;
            }

            // If the player already exists in the round leagueData, remove them from their current position
            // and remove their name from the previous match card.
            if (this.RoundData.HasPlayer(name)) {
                var existing = this.RoundData.PollPlayer(name);
                this.RoundData.RemovePlayer(name);
            }

            this.RoundData.SetPlayer(name, lane, team, pos);
            return true;
        }

        private void SyncRoundData(EventData eventData) {
            foreach (RoundData roundData in this.EventData.Rounds) {
                this.SyncRoundData(roundData, eventData);
            }
        }

        /// <summary>
        /// Synchronizes the provided <paramref name="roundData"/> with the current event settings.
        /// <para>
        /// - Removes empty lanes if there are more lanes than specified in <see cref="EventData.LaneCount"/>.
        /// - Adds new empty lanes if there are fewer lanes than <see cref="EventData.LaneCount"/>.
        /// - Ensures all <see cref="MatchData.Lane"/> values are set to their correct indices.
        /// - Sets the ends for unplayed matches to <see cref="EventData.DefaultEnds"/>.
        /// - Updates the match format for empty matches to match <see cref="EventData.MatchFormat"/>.
        /// </para>
        /// </summary>
        /// <param name="roundData">The round leagueData to update, representing a collection of matches for a round.</param>        
        private void SyncRoundData(RoundData roundData, EventData eventData) {
            // UpdateRound leagueData by removing empty lanes until the number of lanes matches the event leagueData's lane count.
            for (int i = roundData.Count - 1; i >= 0; i--) {
                if (roundData.Count <= eventData.LaneCount) break;
                if (roundData[i].CountPlayers() != 0) continue;
                roundData.RemoveAt(i);
            }

            // If the number of lanes is less than the event leagueData's lane count, add new empty lanes.
            while (roundData.Count < eventData.LaneCount) {
                roundData.Add(new MatchData(eventData.MatchFormat) {
                    Lane = roundData.Count + 1
                });
            }

            // Ensure all match leagueData lanes are set correctly.
            for (int i = 0; i < roundData.Count; i++) {
                roundData[i].Lane = i;
            }

            // Change unplayed match ends to the event's default ends value.
            foreach (MatchData matchData in roundData) {
                if (matchData.Score.Sum() != 0) continue; // If the match has a score, it is played.
                matchData.Ends = eventData.DefaultEnds;
            }

            // Ensure all empty match leagueData has the same match format as the event.
            foreach (MatchData matchData in roundData) {
                if (matchData.CountPlayers() != 0) continue;
                matchData.MatchFormat = eventData.MatchFormat;
            }
        }

        private RoundData GenerateRound() {
            switch (this.EventData.EventType) {
                case EventType.RankedLadder:
                    var newRound = new RankedLadder(this.EventData.Rounds, this.EventData).GenerateRound();
                    return newRound;
                //case EventType.RoundRobin:
                //    break;
                //case EventType.Motley:
                //    break;
                default:
                    throw new NotSupportedException($"Match format '{this.EventData.MatchFormat}' is not supported.");
            }
        }
        internal void DragEndHnd(object sender, RoutedEventArgs e) {
            if (e is not DragEndArgs args) return;
            Debug.WriteLine("MainController.DragEndHnd called.");
            Debug.WriteLine($"From: {args.From.Name}, To: {args.To.Name}");

            switch (args.From) {
                case TeamCard teamCard: {
                        if (args.To is TeamCard target) {
                            TeamData from = this.RoundData[teamCard.MatchCard.Lane].Teams[teamCard.TeamIndex];
                            TeamData to = this.RoundData[target.MatchCard.Lane].Teams[target.TeamIndex];

                            this.RoundData[target.MatchCard.Lane].Teams[target.TeamIndex] = from;
                            this.RoundData[teamCard.MatchCard.Lane].Teams[teamCard.TeamIndex] = to;

                            this.InvokeRoundEvent("Update");
                        }
                    }
                    break;
                case InfoCard infoCard: {
                        if (args.To is TeamCard tcTarget) {

                        }
                        else if (args.To is InfoCard icTarget) {
                        }
                    }
                    break;
            }
        }
    }
}
