using Leagueinator.GUI.Controllers.Algorithms;
using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Forms;
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
using static Leagueinator.GUI.Controls.MatchCard;
using static Leagueinator.GUI.Forms.Main.MainWindow;

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

        private RoundDataCollection RoundDataCollection { get; set; } = [];

        private RoundData RoundData { get => this.RoundDataCollection[this.CurrentRoundIndex]; }

        private int CurrentRoundIndex { get; set; } = 0;

        private EventData EventData { get; set; } = new();

        private string FileName { get; set; } = "Leagueinator"; 

        private bool IsSaved { get; set; } = true;

        private MainWindow Window { get; set; }

        public MainController(MainWindow window) {
            this.Window = window;
            this.NewEvent();
        }

        public void InvokeRoundEvent(string action) {
            this.OnUpdateRound.Invoke(this, new(action, this.CurrentRoundIndex, this.RoundData));
        }

        public void InvokeRoundEvent(string action, int index) {
            this.OnUpdateRound.Invoke(this, new(action, index, this.RoundDataCollection[index]));
        }

        public void InvokeRoundEvent(string action, int index, RoundData? roundData) {
            this.OnUpdateRound.Invoke(this, new(action, index, roundData));
        }

        public void InvokeSetTitle(string filename, bool saved) {
            this.IsSaved = saved;
            this.OnSetTitle.Invoke(this, filename, saved);
        }   

        public void FileEventHnd(object? sender, FileEventArgs e) {
            Logger.Log($"MainController.FileEvent: {e.Action}");

            switch(e.Action) {
                case "Load":
                    this.Load();
                    this.OnSetRoundCount.Invoke(this, this.RoundDataCollection.Count);
                    this.InvokeRoundEvent("Update");
                    this.InvokeSetTitle(this.FileName, true);
                    break;
                case "Save":
                    if (this.FileName != "Leagueinator") {
                        this.Save(this.FileName);
                    }
                    else {
                        this.SaveAs();
                    }
                    this.InvokeSetTitle(this.FileName, true);

                    break;
                case "SaveAs":
                    this.SaveAs();
                    this.InvokeSetTitle(this.FileName, true);
                    break;  
                case "New":
                    this.NewEvent();
                    this.OnSetRoundCount.Invoke(this, this.RoundDataCollection.Count);
                    this.InvokeRoundEvent("Update");
                    this.InvokeSetTitle("Leagueinator", true);
                    this.FileName = "Leagueinator";
                    break;
                default:
                    throw new NotSupportedException($"Action '{e.Action}' is not supported.");
            }
        }
        internal void ActionEventHnd(object? sender, ActionEventArg e) {
            Logger.Log($"MainController.ActionEventHnd: {e.Action}");

            switch (e.Action) {
                case "Settings":
                    var dialog = new SettingsDialog() {
                        EventData = this.EventData,
                    };

                    if (dialog.ShowDialog() == true) {
                        Debug.WriteLine("SettingsDialog confirmed changes.");
                        this.EventData = dialog.EventData;
                        this.SyncRoundData(dialog.EventData);
                        this.InvokeRoundEvent("Update");
                        this.InvokeSetTitle(this.FileName, false);
                    }
                    break;
                case "PrintTeams":
                    this.PrintTeams();
                    break;
                default:
                    throw new NotSupportedException($"Action '{e.Action}' is not supported.");
            }
        }

        public void PrintTeams() {
            PrintWindow pw = new(this.RoundDataCollection);
            pw.Show();
        }

        public void RoundDataHnd(object? sender, RoundDataEventArgs e) {
            Logger.Log($"MainController.RoundData: {e.Action} for index {e.Index}");

            try {
                this._RoundDataHnd(sender, e);
            }
            catch (UnpairableTeamsException ex) {
                MessageBox.Show("Could not assign teams uniquely.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (AlgoLogicException ex) {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void _RoundDataHnd(object? sender, RoundDataEventArgs e) {
            switch (e.Action) {
                case "AssignLanes": {
                        AssignLanes assignLanes = new(this.EventData, this.RoundDataCollection, this.RoundData);
                        RoundData newRound = assignLanes.DoAssignment();
                        this.RoundDataCollection[this.CurrentRoundIndex] = newRound;
                        this.InvokeSetTitle(this.FileName, false);
                        this.InvokeRoundEvent("Update");
                    } 
                    break;
                case "GenerateRound": {
                        var newRound = this.GenerateRound();
                        AssignLanes assignLanes = new(this.EventData, this.RoundDataCollection, newRound);
                        newRound = assignLanes.DoAssignment();  
                        this.RoundDataCollection.Add(newRound);
                        this.CurrentRoundIndex = this.RoundDataCollection.Count - 1;

                        this.InvokeRoundEvent("AddRound", this.CurrentRoundIndex, newRound);
                        this.InvokeSetTitle(this.FileName, false);
                        this.InvokeRoundEvent("Update");
                    }
                    break;
                case "AddRound": {
                        RoundData newRound = new(this.EventData);
                        this.RoundDataCollection.Add(newRound);
                        var createdIndex = this.RoundDataCollection.Count - 1;
                        this.InvokeRoundEvent("AddRound", createdIndex, newRound);
                        this.InvokeSetTitle(this.FileName, false);
                    }
                    break;
                case "Remove":
                    var previousIndex = this.CurrentRoundIndex;                    
                    this.RemoveRound(this.CurrentRoundIndex);
                    this.InvokeRoundEvent("Update");
                    this.InvokeRoundEvent("RemoveRound", previousIndex, null);
                    this.InvokeSetTitle(this.FileName, false);
                    break;
                case "Select":
                    if (e.Index == -1) {
                        this.CurrentRoundIndex = this.RoundDataCollection.Count - 1;
                    }
                    else {
                        this.CurrentRoundIndex = e.Index;
                    }                        
                    this.InvokeRoundEvent("Update");
                    break;
                case "Copy":
                    this.RoundDataCollection.Add(this.RoundData.Copy());
                    var copiedIndex = this.RoundDataCollection.Count - 1;
                    this.InvokeRoundEvent("AddRound", copiedIndex);
                    this.InvokeSetTitle(this.FileName, false);
                    break;
                case "AssignPlayersRandomly":
                    this.RoundData.AssignPlayersRandomly();
                    this.InvokeRoundEvent("Update");
                    this.InvokeSetTitle(this.FileName, false);
                    break;
                case "AssignRankedLadder":
                    RankedLadder ladder = new(this.RoundDataCollection, this.EventData);
                    var nextRound = ladder.GenerateRound();
                    Debug.WriteLine($"Generated next round with {nextRound.Count} matches.");
                    Debug.WriteLine(nextRound);
                    break;
                case "AssignRoundRobin":
                    break;
                case "RoundResults": {
                        RoundResults rr = new(this.RoundData);
                        TableViewer tv = new TableViewer();

                        foreach (SingleResult result in rr.Results) {
                            tv.Append(result.ToString());
                        }

                        tv.Show();
                    }
                    break;
                case "EventResults": {
                        EventResults er = new(this.RoundDataCollection);
                        TableViewer tv = new TableViewer();

                        foreach (TeamResult result in er.ResultsByTeam) {
                            tv.Append(result.ToString());
                        }

                        tv.Show();
                    }
                    break;
                case "Show": {
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

                        foreach (RoundData round in this.RoundDataCollection) {
                            tv.Append($"Round {this.RoundDataCollection.IndexOf(round) + 1}:");
                            tv.Append(round);
                        }
                        tv.Show();
                    }
                    break;
                default:
                    throw new NotSupportedException($"Action '{e.Action}' is not supported.");
            }
        }

        private RoundData GenerateRound() {
            switch(this.EventData.EventType) {
                case EventType.RankedLadder:
                    var newRound = new RankedLadder(this.RoundDataCollection, this.EventData).GenerateRound();
                    return newRound;
                //case EventType.RoundRobin:
                //    break;
                //case EventType.Motley:
                //    break;
                default:
                    throw new NotSupportedException($"Match format '{this.EventData.MatchFormat}' is not supported.");
            }
        }

        public void MatchCardUpdateHnd(object sender, RoutedEventArgs e) {
            if (e is not MatchCardEventArgs args) return;
            Logger.Log($"MainController.MatchCardUpdate: {args.Field} for lane {args.Lane}");

            switch (args.Field) {
                case "Name":
                    if (e is not MatchCardNewArgs nameArgs) return;
                    if (this.UpdateName(nameArgs)) {
                        this.InvokeRoundEvent("Update");
                        this.InvokeSetTitle(this.FileName, false);
                    }
                    break;
                case "Ends":
                    if (e is not MatchCardNewArgs endsArgs) return;
                    this.RoundData[endsArgs.Lane].Ends = (int)endsArgs.NewValue;
                    this.InvokeRoundEvent("Update");
                    this.InvokeSetTitle(this.FileName, false);
                    break;
                case "Tie":
                    if (e is not MatchCardNewArgs tieArgs) return;
                    this.RoundData[tieArgs.Lane].TieBreaker = (int)tieArgs.NewValue;
                    this.InvokeRoundEvent("Update");
                    this.InvokeSetTitle(this.FileName, false);
                    break;
                case "Bowls":
                    if (e is not MatchCardNewArgs bowlsArgs) return;
                    if (bowlsArgs.Team is null) throw new ArgumentException("Team index must be provided for bowls changes.");
                    this.RoundData[bowlsArgs.Lane].Score[(int)bowlsArgs.Team] = (int)bowlsArgs.NewValue;
                    this.InvokeRoundEvent("Update");
                    this.InvokeSetTitle(this.FileName, false);
                    break;
                case "Format":
                    if (e is not MatchCardNewArgs formatArgs) return;
                    this.RoundData[args.Lane].MatchFormat = (MatchFormat)formatArgs.NewValue;
                    this.InvokeRoundEvent("Update");
                    this.InvokeSetTitle(this.FileName, false);
                    break;
                case "Remove":
                    this.RoundData.RemoveAt(args.Lane);
                    if (this.CurrentRoundIndex == args.Lane) {
                        this.CurrentRoundIndex = Math.Max(0, this.CurrentRoundIndex - 1);
                    }
                    this.InvokeRoundEvent("RemoveMatch", args.Lane);
                    this.InvokeSetTitle(this.FileName, false);
                    break;
            }
        }

        private void NewEvent() {
            if (!this.IsSaved) {
                ConfirmationDialog confDialog = new() {
                    Owner = this.Window,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Text = "League not saved. Do you still want to create a new one?"
                };

                if (confDialog.ShowDialog() == false) return;
            }

            this.EventData = new EventData();
            this.RoundDataCollection = [];
            RoundData newRound = new(this.EventData);
            this.RoundDataCollection.Add(newRound);
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
                ModelSaveData data = JsonSerializer.Deserialize<ModelSaveData>(contents, this.GetOptions())
                    ?? throw new InvalidDataException("Failed to deserialize the file contents.");

                this.RoundDataCollection = data.RoundDataCollection;
                this.CurrentRoundIndex = 0;
                this.EventData = data.EventData;
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
            ModelSaveData data = new(this.EventData, this.RoundDataCollection);

            // Serialize the EventData and RoundCollection to JSON and write to the file.
            StreamWriter writer = new(filename);
            string json = JsonSerializer.Serialize(data, this.GetOptions());
            writer.WriteLine(json);
            writer.Close();

            this.FileName = filename;
            this.IsSaved = true;
        }

        private void RemoveRound(int index) {
            this.RoundDataCollection.RemoveAt(index);
            
            if (this.RoundDataCollection.Count == 0) {
                this.RoundDataCollection.Add(new RoundData(this.EventData));
                this.CurrentRoundIndex = 0;
            } else if (this.CurrentRoundIndex >= this.RoundDataCollection.Count) {
                this.CurrentRoundIndex = this.RoundDataCollection.Count - 1;
            }
            Debug.WriteLine($"Removed round at index {index}. Current round index is now {this.CurrentRoundIndex}.");
        }

        private bool UpdateName(MatchCardNewArgs args) {
            if (args.Team is null) throw new ArgumentException("Team index must be provided for player name changes.");
            if (args.Position is null) throw new ArgumentException("Position must be provided for player name changes.");

            var name = (string)args.NewValue;
            var lane = args.Lane;
            var team = (int)args.Team;
            var pos = (int)args.Position;

            if (name == string.Empty && this.RoundData[lane].Teams[team][pos] == string.Empty) {
                // If the name is empty and the player is already empty, do nothing.
                return false;
            }

            var poll = this.RoundData.PollPlayer(name);
            if (poll == (lane, team, pos) ) {
                return false;
            }

            // If the player already exists in the round data, remove them from their current position
            // and remove their name from the previous match card.
            if (this.RoundData.HasPlayer(name)) {
                var existing = this.RoundData.PollPlayer(name);
                this.RoundData.RemovePlayer(name);
            }

            this.RoundData.SetPlayer((string)args.NewValue, args.Lane, (int)args.Team, (int)args.Position);
            return true;
        }

        private void SyncRoundData(EventData eventData) {
            foreach (RoundData roundData in this.RoundDataCollection) {
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
        /// <param name="roundData">The round data to update, representing a collection of matches for a round.</param>        
        private void SyncRoundData(RoundData roundData, EventData eventData) {
            // UpdateRound data by removing empty lanes until the number of lanes matches the event data's lane count.
            for (int i = roundData.Count - 1; i >= 0; i--) {
                if (roundData.Count <= eventData.LaneCount) break;
                if (roundData[i].CountPlayers() != 0) continue;
                roundData.RemoveAt(i);
            }

            // If the number of lanes is less than the event data's lane count, add new empty lanes.
            while (roundData.Count < eventData.LaneCount) {
                roundData.Add(new MatchData(eventData.MatchFormat) {
                    Lane = roundData.Count + 1
                });
            }

            // Ensure all match data lanes are set correctly.
            for (int i = 0; i < roundData.Count; i++) {
                roundData[i].Lane = i;
            }

            // Change unplayed match ends to the event's default ends value.
            foreach (MatchData matchData in roundData) {
                if (matchData.Score.Sum() != 0) continue; // If the match has a score, it is played.
                matchData.Ends = eventData.DefaultEnds;
            }

            // Ensure all empty match data has the same match format as the event.
            foreach (MatchData matchData in roundData) {
                if (matchData.CountPlayers() != 0) continue;
                matchData.MatchFormat = eventData.MatchFormat;
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
