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
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Windows;
using System.Windows.Documents;

namespace Leagueinator.GUI.Controllers {

    /// <summary>
    /// This glues the MainWindow window to the model and handles events from the MainWindow window.
    /// </summary>
    public class MainController {
        public NamedEventReceiver NamedEventRcv { get; private set; }

        public NamedEventDispatcher NamedEventDisp { get; set; }

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
            this.NamedEventDisp = new(this);
            this.Window = window;
            this.NewLeague();
        }

        public void InvokeRoundUpdate() {
            this.NamedEventDisp.Dispatch(EventName.RoundUpdated, new() {
                ["roundIndex"] = this.CurrentRoundIndex,
                ["roundData"] = this.RoundData.AsReadOnly()
            });
        }

        public void InvokeAddRound(RoundData roundData) {
            this.NamedEventDisp.Dispatch(EventName.RoundAdded, new() {
                ["roundData"] = roundData.AsReadOnly()
            });
        }

        public void InvokeSetTitle(string title, bool saved) {
            this.IsSaved = saved;
            this.NamedEventDisp.Dispatch(EventName.SetTitle, new() {
                ["title"] = title,
                ["saved"] = saved
            });
        }

        [NamedEventHandler(EventName.SelectEvent)]
        internal void DoSelectEvent(int eventUID) {
            EventData eventData = this.LeagueData.GetEvent(eventUID);
        }

        [NamedEventHandler(EventName.AddEvent)]
        internal void DoAddEvent() {
            this.LeagueData.AddEvent();
        }

        [NamedEventHandler(EventName.EventManager)]
        internal void DoEventManager() {
            var form = new EventManagerForm();
            form.NamedEventDisp += this.NamedEventRcv;
            this.NamedEventDisp += form.NamedEventRcv;
            form.ShowDialog(this, this.LeagueData.AsReadOnly());
        }

        [NamedEventHandler(EventName.LoadLeague)]
        internal void DoLoad() {
            this.Load();
            this.NamedEventDisp.Dispatch(EventName.UpdateRoundCount, new() {
                ["count"] = this.EventData.Rounds.Count
            });
            this.InvokeRoundUpdate();
            this.InvokeSetTitle(this.FileName, true);
        }

        [NamedEventHandler(EventName.RenameEvent)]
        internal void DoRenameEvent(string name, int uid) {
            foreach (EventData eventData in this.LeagueData) {
                if (eventData.UID == uid) {
                    eventData.EventName = name;
                    return;
                }
            }
        }

        [NamedEventHandler(EventName.CreateEvent)]
        internal void DoCreateEvent() {
            EventData eventData = this.LeagueData.AddEvent();
            this.NamedEventDisp.Dispatch(EventName.EventAdded, new() {
                ["uid"] = eventData.UID,

            });
        }

        [NamedEventHandler(EventName.SaveLeague)]
        internal void DoSave() {
            if (this.FileName != "Leagueinator") {
                this.Save(this.FileName);
            }
            else {
                this.SaveAs();
            }
            this.InvokeSetTitle(this.FileName, true);
        }

        [NamedEventHandler(EventName.SaveLeagueAs)]
        internal void DoSaveAs() {
            this.SaveAs();
            this.InvokeSetTitle(this.FileName, true);
        }

        [NamedEventHandler(EventName.NewLeague)]
        internal void DoNew() {
            this.NewLeague();

            this.NamedEventDisp.Dispatch(EventName.UpdateRoundCount, new() {
                ["count"] = this.EventData.Rounds.Count
            });

            this.InvokeRoundUpdate();
            this.InvokeSetTitle("Leagueinator", true);
            this.FileName = "Leagueinator";
        }

        [NamedEventHandler(EventName.PrintTeams)]
        internal void DoPrintTeams() {
            PrintWindow pw = new(this.EventData.Rounds);
            pw.Show();
        }

        [NamedEventHandler(EventName.AssignLanes)]
        internal void DoAssignLanes() {
            AssignLanes assignLanes = new(this.EventData, EventData.Rounds, this.RoundData); // TODO can just pass this.EventData
            RoundData newRound = assignLanes.DoAssignment();
            this.EventData.Rounds.Add(newRound);
            this.InvokeSetTitle(this.FileName, false);
            this.InvokeRoundUpdate();
        }

        [NamedEventHandler(EventName.GenerateRound)]
        internal void DoGenerateRound() {
            var newRound = this.GenerateRound();
            AssignLanes assignLanes = new(this.EventData, this.EventData.Rounds, newRound); // TODO can just pass this.EventData
            newRound = assignLanes.DoAssignment();
            this.EventData.Rounds.Add(newRound);
            this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
            this.InvokeAddRound(newRound);
            this.InvokeSetTitle(this.FileName, false);
            this.InvokeRoundUpdate();
        }

        [NamedEventHandler(EventName.AddRound)]
        internal void DoAddRound() {
            RoundData newRound = new(this.EventData);
            this.EventData.Rounds.Add(newRound);
            this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
            this.InvokeAddRound(newRound);
            this.InvokeSetTitle(this.FileName, false);
            this.InvokeRoundUpdate();
        }

        [NamedEventHandler(EventName.RemoveRound)]
        internal void DoRemoveRound() {
            var removedIndex = this.CurrentRoundIndex;
            this.RemoveRound(this.CurrentRoundIndex);
            this.InvokeRoundUpdate();
            this.InvokeSetTitle(this.FileName, false);

            this.NamedEventDisp.Dispatch(EventName.RoundRemoved, new() {
                ["roundIndex"] = removedIndex
            });
        }

        [NamedEventHandler(EventName.SelectRound)]
        internal void DoSelectRound(int index = -1) {

            if (index == -1) {
                this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
            }
            else {
                this.CurrentRoundIndex = index;
            }
            this.InvokeRoundUpdate();
        }

        [NamedEventHandler(EventName.CopyRound)]
        internal void DoCopyRound() {
            RoundData newRound = this.RoundData.Copy();
            this.EventData.Rounds.Add(newRound);
            var copiedIndex = this.EventData.Rounds.Count - 1;
            this.InvokeAddRound(newRound);
            this.InvokeSetTitle(this.FileName, false);
        }

        [NamedEventHandler(EventName.AssignPlayersRandomly)]
        internal void DoAssignPlayersRandomly() {
            this.RoundData.AssignPlayersRandomly();
            this.InvokeRoundUpdate();
            this.InvokeSetTitle(this.FileName, false);
        }

        [NamedEventHandler(EventName.DisplayRoundResults)]
        internal void DoRoundResults() {
            RoundResults rr = new(this.RoundData);
            TableViewer tv = new TableViewer();

            foreach (SingleResult result in rr.Results) {
                tv.Append(result.ToString());
            }

            tv.Show();
        }

        [NamedEventHandler(EventName.DisplayEventResults)]
        internal void DoEventResults(object? sender, NamedEventArgs e) {
            EventResults er = new(this.EventData.Rounds);
            TableViewer tv = new TableViewer();
            foreach (TeamResult result in er.ResultsByTeam) {
                tv.Append(result.ToString());
            }
            tv.Show();
        }

        [NamedEventHandler(EventName.ShowData)]
        internal void DoShow() {
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

        [NamedEventHandler(EventName.ChangePlayerName)]
        internal void DoPlayerName(string name, int lane, int teamIndex, int position) {

            if (this.UpdateName(name, lane, teamIndex, position)) {
                this.InvokeRoundUpdate();
                this.InvokeSetTitle(this.FileName, false);
            }
        }

        [NamedEventHandler(EventName.ChangeEnds)]
        internal void DoEnds(int lane, int ends) {

            this.RoundData[lane].Ends = ends;
            this.InvokeSetTitle(this.FileName, false);
            this.NamedEventDisp.Dispatch(EventName.EndsUpdated, new() {
                ["lane"] = lane,
                ["ends"]= ends,
            });
        }

        [NamedEventHandler(EventName.ChangeTieBreaker)]
        internal void DoTieBreaker(int lane, int tieBreaker) {
            this.RoundData[lane].TieBreaker = tieBreaker;
            this.InvokeSetTitle(this.FileName, false);
            this.NamedEventDisp.Dispatch(EventName.TieBreakerUpdated, new() {
                ["lane"] = lane,
                ["tieBreaker"] = tieBreaker,
            });
        }

        [NamedEventHandler(EventName.ChangeBowls)]
        internal void DoBowls(int lane, int teamIndex, int bowls) {
            this.RoundData[lane].Score[teamIndex] = bowls;            
            this.InvokeSetTitle(this.FileName, false);

            this.NamedEventDisp.Dispatch(EventName.BowlsUpdated, new() {
                ["lane"] = lane,
                ["teamIndex"] = teamIndex,
                ["bowls"] = bowls
            });
        }

        [NamedEventHandler(EventName.ChangeMatchFormat)]
        internal void DoMatchFormat(int lane, MatchFormat format) {
            this.RoundData[lane].MatchFormat = format;
            this.InvokeRoundUpdate();
            this.InvokeSetTitle(this.FileName, false);
        }

        [NamedEventHandler(EventName.RemoveMatch)]
        internal void DoRemoveMatch(int lane) {
            this.RoundData.RemoveAt(lane);
            this.NamedEventDisp.Dispatch(EventName.MatchRemoved, new() {
                ["lane"] = lane
            });
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
            this.LeagueData.AddEvent();
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
            // RoundUpdated leagueData by removing empty lanes until the number of lanes matches the event leagueData's lane count.
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

                            this.InvokeRoundUpdate();
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
