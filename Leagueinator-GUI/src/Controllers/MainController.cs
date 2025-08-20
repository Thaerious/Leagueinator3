using Leagueinator.GUI.Controllers.Modules;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Forms;
using Leagueinator.GUI.Forms.Event;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Leagueinator.GUI.Model.ViewModel;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Utility;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers {

    /// <summary>
    /// This glues the MainWindow window to the model and handles _events from the MainWindow window.
    /// </summary>
    public partial class MainController {

        #region Properties
        internal LeagueData LeagueData { get; private set; } = new();

        private EventData? _eventData = default;

        private IModule? EventTypeModule = null;

        internal EventData EventData {
            get {
                if (_eventData == null) throw new NullReferenceException("EventData not set.");
                return _eventData;
            }
            set {
                this._eventData = value;
                if (value != null) this.UpdateModules(value.EventType);
            }
        }

        private void UpdateModules(EventType eventType) {
            if (this.EventTypeModule != null) {
                this.EventTypeModule!.UnloadModule();
            }

            this.EventTypeModule = eventType.GetModule();
            this.EventTypeModule.LoadModule(this.Window, this);
        }

        private int CurrentRoundIndex { get; set; } = 0;

        internal RoundData RoundData => this.EventData.Rounds[this.CurrentRoundIndex];

        private string Title { get; set; } = "Leagueinator";

        private bool IsSaved { get; set; } = true;

        private MainWindow Window { get; set; }

        #endregion

        public MainController(MainWindow window) {
            this.Window = window;
            NamedEvent.RegisterHandler(this);
        }

        #region Dispatch Methods

        public void DispatchModel(EventName eventName = EventName.SetModel) {
            this.DispatchEvent(eventName, new() {
                ["eventNames"] = this.LeagueData.Events.Select(e => e.EventName).ToList(),
                ["selectedEvent"] = EventData.EventName,
                ["roundIndex"] = this.CurrentRoundIndex,
                ["matchRecords"] = MatchRecord.MatchRecordList(this.RoundData),
                ["playerRecords"] = this.RoundData.Records().ToList(),
                ["roundCount"] = this.EventData.Count(),
                ["nameAlerts"] = this.BuildNameAlerts(),
                ["laneAlerts"] = this.BuildLaneAlerts(),
            });
        }

        /// <summary>
        /// Build a list of lanes that have players that have played on the lane before.
        /// </summary>
        /// <returns></returns>
        private HashSet<int> BuildLaneAlerts() {
            HashSet<int> lanes = [];
            DefaultDictionary<string, HashSet<int>> hasPlayed = new(key => new());

            foreach (RoundData round in this.EventData.Rounds) {
                if (round == this.RoundData) continue;
                foreach (PlayerRecord record in round.Records()) {
                    hasPlayed[record.Name].Add(record.Lane);
                }
            }

            foreach (PlayerRecord record in this.RoundData.Records()) {
                if (hasPlayed[record.Name].Contains(record.Lane)) {
                    lanes.Add(record.Lane);
                }
            }

            return lanes;
        }

        private List<string> BuildNameAlerts() {
            // Collect all names that are not in the current round
            HashSet<string> roster = [..this.LeagueData.Events
                                            .SelectMany(e => e.Rounds)
                                            .Where(r => r != this.RoundData)
                                            .SelectMany(r => r.AllNames())];

            // Return the symmetric difference of the two sets
            return this.RoundData.AllNames().Except(roster).ToList();
        }

        public void DispatchSetTitle(bool saved) {
            this.DispatchSetTitle(this.Title, saved);
        }

        public void DispatchSetTitle(string title, bool saved) {
            this.IsSaved = saved;
            this.Title = title;

            this.DispatchEvent(EventName.SetTitle, new() {
                ["title"] = title,
                ["saved"] = saved
            });
        }

        #endregion

        #region Event Handlers
        [NamedEventHandler(EventName.RenameEvent)]
        internal void DoRenameEvent(string from, string to) {
            if (this.LeagueData.Events.Select(e => e.EventName).Where(name => name == to).Any()) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["alertLevel"] = AlertLevel.Inform,
                    ["message"] = $"Event name '{to}' already exits."
                });
                return;
            }

            if (!this.LeagueData.Events.Select(e => e.EventName).Where(name => name == from).Any()) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["alertLevel"] = AlertLevel.Inform,
                    ["message"] = $"Event name '{from}' does not exist."
                });
                return;
            }


            this.LeagueData.Events
                .Where(e => e.EventName == from)
                .First()
                .EventName = to;
        }

        [NamedEventHandler(EventName.SelectEvent)]
        internal void DoSelectEvent(int index) {
            this.EventData = this.LeagueData.Events[index];
            this.CurrentRoundIndex = this.EventData.Count() - 1;
            this.DispatchModel(EventName.EventSelected);
        }

        [NamedEventHandler(EventName.AddEvent)]
        internal void DoAddEvent() {
            EventData eventData = this.LeagueData.AddEvent();
            eventData.DefaultEnds = this.EventData.DefaultEnds;
            eventData.LaneCount = this.EventData.LaneCount;
            eventData.DefaultMatchFormat = this.EventData.DefaultMatchFormat;
            eventData.EventType = this.EventData.EventType;
            eventData.AddRound();

            this.EventData = eventData;
            this.CurrentRoundIndex = this.EventData.Count() - 1;

            this.DispatchModel(EventName.SetModel);
        }

        [NamedEventHandler(EventName.DeleteEvent)]
        internal void DoDeleteEvent(string eventName) {
            if (this.LeagueData.Events.Count < 2) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["alertLevel"] = AlertLevel.Inform,
                    ["message"] = "Can not delete last event."
                });
                return;
            }

            EventData eventData = this.LeagueData.Events.Where(e => e.EventName == eventName).First();
            this.LeagueData.RemoveEvent(eventData);
            this.EventData = this.LeagueData.Events[this.LeagueData.Events.Count - 1];
            this.CurrentRoundIndex = this.EventData.Count() - 1;
            this.DispatchModel(EventName.SetModel);
        }

        [NamedEventHandler(EventName.ShowEventSettings)]
        internal void DoShowEventSettings(string eventName) {
            var form = new EventSettingsForm() {
                Owner = this.Window,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            form.PauseEvents();
            NamedEvent.RegisterHandler(form);

            EventData eventData = this.LeagueData.Events.Where(e => e.EventName == eventName).First();

            if (form.ShowDialog(eventData) ?? false) {
                this.DispatchModel(EventName.SetModel);

                eventData.EventName = form.TxtName.Text;
                eventData.DefaultEnds = int.Parse(form.TxtEnds.Text);
                eventData.LaneCount = int.Parse(form.TxtLanes.Text);
                eventData.DefaultMatchFormat = form.MatchFormat;
                eventData.EventType = form.EventType;

                this.FixEnds(eventData.DefaultEnds);
                this.FixLanes(eventData.LaneCount);
                this.FixFormat(eventData.DefaultMatchFormat);
                this.DispatchModel(EventName.SetModel);

                this.UpdateModules(eventData.EventType);
            }

            NamedEvent.RemoveHandler(form);
        }

        private void FixEnds(int value) {
            // Each match w/o a player has it's ends changed to the new value.
            this.EventData.Rounds
                .SelectMany(r => r.Matches)
                .Where(match => match.AllNames().Count == 0)
                .ToList()
                .ForEach(match => match.Ends = value);
        }

        private void FixLanes(int value) {
            // RemovePlayer empty matches until the lane count matches.
            foreach (RoundData round in this.EventData.Rounds) {
                while (round.Matches.Count > value) {
                    MatchData? match = round.Matches.Where(m => m.CountPlayers() == 0).FirstOrDefault();
                    if (match != null) {
                        round.RemoveMatch(match);
                    }
                    else {
                        this.DispatchEvent(EventName.Notification, new() {
                            ["alertLevel"] = AlertLevel.Warning,
                            ["message"] = $"Removing matches with players."
                        });
                        break;
                    }
                }
            }

            // RemovePlayer any match until the lane count matches.
            foreach (RoundData round in this.EventData.Rounds) {
                while (round.Matches.Count > value) {
                    round.RemoveMatch(round.Matches[0]);
                }
            }

            // Add matches until the round has enough lanes.
            foreach (RoundData round in this.EventData.Rounds) {
                round.Fill();
            }
        }

        private void FixFormat(MatchFormat defaultMatchFormat) {
            foreach (RoundData round in this.EventData.Rounds) {
                foreach (MatchData match in round.Matches) {
                    if (match.CountPlayers() == 0) {
                        match.MatchFormat = defaultMatchFormat;
                    }
                }
            }
        }

        #endregion

        #region League Handlers

        [NamedEventHandler(EventName.LoadLeague)]
        internal void DoLoad() {
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
                StreamReader reader = new StreamReader(dialog.FileName);
                this.LeagueData = LeagueData.ReadIn(reader);
                this.EventData = this.LeagueData.Events[^1];
                this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
                this.DispatchSetTitle(dialog.FileName, true);
                this.DispatchModel(EventName.SetModel);
            }
        }

        [NamedEventHandler(EventName.SaveLeague)]
        internal void DoSave() {
            if (this.Title != "Leagueinator") {
                this.Save(this.Title);
            }
            else {
                this.SaveAs();
            }
        }

        [NamedEventHandler(EventName.SaveLeagueAs)]
        internal void DoSaveAs() {
            this.SaveAs();
            this.DispatchSetTitle(this.Title, true);
        }

        [NamedEventHandler(EventName.NewLeague)]
        internal void DoNew() {
            this.NewLeague();
            this.DispatchModel(EventName.SetModel);
            this.DispatchSetTitle("Leagueinator", true);
            this.Title = "Leagueinator";
            this.DispatchSetTitle(this.Title, true);
        }
        #endregion

        #region Round Handlers

        [NamedEventHandler(EventName.AddRound)]
        internal void DoAddRound() {
            RoundData newRound = this.EventData.AddRound();
            this.AddRound(newRound);
        }

        [NamedEventHandler(EventName.DeleteRound)]
        internal void DoDeleteRound(int? roundIndex = null) {
            int index = roundIndex ?? this.CurrentRoundIndex;
            this.RemoveRound(index);
            this.DispatchModel(EventName.RoundDeleted);
            this.DispatchSetTitle(this.Title, false);
        }

        [NamedEventHandler(EventName.SelectRound)]
        internal void DoSelectRound(int index = -1) {

            if (index == -1) {
                this.CurrentRoundIndex = this.EventData.Rounds.Count - 1;
            }
            else {
                this.CurrentRoundIndex = index;
            }
            this.DispatchModel(EventName.RoundChanged);
        }

        [NamedEventHandler(EventName.CopyRound)]
        internal void DoCopyRound() {
            RoundData newRound = this.RoundData.Copy();
            Debug.WriteLine(newRound);
            this.EventData.AddRound(newRound);
            this.AddRound(newRound);
            this.DispatchSetTitle(this.Title, false);
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

        #region Match, Team, Player Handlers  
        [NamedEventHandler(EventName.ChangePlayerName)]
        internal void DoChangePlayerName(string name, int lane, int teamIndex, int position) {
            if (this.RoundData.Matches[lane].Teams[teamIndex].Players[position].Equals(name)) {
                return;
            }

            name = name.Trim();

            if (this.RoundData.HasPlayer(name)) {
                var record = this.RoundData.Records().Where(r => r.Name == name).First();
                this.RoundData.RemovePlayer(name);

                this.DispatchEvent(EventName.NameUpdated, new() {
                    ["lane"] = record.Lane,
                    ["teamIndex"] = record.Team,
                    ["position"] = record.PlayerPos,
                    ["name"] = "",
                    ["nameAlerts"] = this.BuildNameAlerts(),
                    ["laneAlerts"] = this.BuildLaneAlerts(),
                });
            }

            this.RoundData.Matches[lane].Teams[teamIndex].SetPlayer(position, name);
            this.DispatchEvent(EventName.NameUpdated, new() {
                ["lane"] = lane,
                ["teamIndex"] = teamIndex,
                ["position"] = position,
                ["name"] = name,
                ["nameAlerts"] = this.BuildNameAlerts(),
                ["laneAlerts"] = this.BuildLaneAlerts(),
            });

            this.DispatchSetTitle(this.Title, false);
        }

        [NamedEventHandler(EventName.ChangeEnds)]
        internal void DoEnds(int lane, int ends) {
            if (lane < 0 || lane >= this.RoundData.Matches.Count) {
                throw new ArgumentOutOfRangeException($"Argument '{nameof(lane)}' value '{lane}' is not within [0 .. {this.RoundData.Matches.Count - 1}]");
            }

            this.RoundData.Matches[lane].Ends = ends;
            this.DispatchEvent(EventName.EndsUpdated, new() {
                ["lane"] = lane,
                ["ends"] = ends,
            });

            this.DispatchSetTitle(this.Title, false);
        }

        [NamedEventHandler(EventName.ChangeTieBreaker)]
        internal void DoTieBreaker(int lane, int teamIndex) {
            this.RoundData.Matches[lane].TieBreaker = teamIndex;
            this.DispatchEvent(EventName.TieBreakerUpdated, new() {
                ["lane"] = lane,
                ["teamIndex"] = teamIndex,
            });

            this.DispatchSetTitle(this.Title, false);
        }

        [NamedEventHandler(EventName.ChangeBowls)]
        internal void DoBowls(int lane, int teamIndex, int bowls) {
            this.RoundData.Matches[lane].Score[teamIndex] = bowls;

            this.DispatchEvent(EventName.BowlsUpdated, new() {
                ["lane"] = lane,
                ["bowls"] = (int[])this.RoundData.Matches[lane].Score.Clone()
            });

            this.DispatchSetTitle(this.Title, false);
        }

        [NamedEventHandler(EventName.ChangeMatchFormat)]
        internal void DoMatchFormat(int lane, MatchFormat format) {
            this.RoundData.Matches[lane].MatchFormat = format;
            this.DispatchModel(EventName.RoundChanged);
            this.DispatchSetTitle(this.Title, false);
        }

        [NamedEventHandler(EventName.SwapTeams)]
        internal void DoSwap(int fromLane, int toLane, int fromIndex, int toIndex) {
            List<string> namesFrom = [.. this.RoundData.Matches[fromLane].Teams[fromIndex].Players];
            List<string> namesTo = [.. this.RoundData.Matches[toLane].Teams[toIndex].Players];

            this.RoundData.Matches[fromLane].Teams[fromIndex].CopyFrom(namesTo);
            this.RoundData.Matches[toLane].Teams[toIndex].CopyFrom(namesFrom);
            this.DispatchModel(EventName.RoundChanged);
            this.DispatchSetTitle(this.Title, false);
        }

        [NamedEventHandler(EventName.SwapMatches)]
        internal void DoSwapMatch(int lane1, int lane2) {
            this.RoundData.SwapMatch(lane1, lane2);
            this.DispatchModel(EventName.RoundChanged);
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

            this.LeagueData = new();
            this.LeagueData.AddEvent();
            this.EventData = this.LeagueData.Events[0];
            RoundData newRound = this.EventData.AddRound();
            this.CurrentRoundIndex = 0;
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
            StreamWriter writer = new(filename);
            this.LeagueData.WriteOut(writer);
            writer.Close();
            this.DispatchSetTitle(filename, true);
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

        private string GetShow() {
            string sb = string.Empty;
            sb += "Controller Data\n";
            sb += $"File Name: {this.Title}\n";
            sb += $"Is Saved: {this.IsSaved}\n";
            sb += $"Current Round Index: {this.CurrentRoundIndex}/{this.EventData.Count()}\n";
            sb += $"No. of Events: {this.LeagueData.Events.Count}\n";
            sb += "\nEvent Data\n";
            sb += $"Event Name: {this.EventData.EventName}\n";
            sb += $"Number of Lanes: {this.EventData.LaneCount}\n";
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
            this.DispatchModel(EventName.RoundAdded);
            this.DispatchSetTitle(this.Title, false);
        }
        #endregion
    }
}
