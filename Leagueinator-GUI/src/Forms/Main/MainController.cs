using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using static Leagueinator.GUI.Controls.MatchCard;

namespace Leagueinator.GUI.Forms.Main {
    public class MainController {

        private List<RoundData> RoundDataCollection { get; } = [];

        private MainWindow MainWindow { get; set; }

        private RoundData RoundData {get; set; }

        public EventData EventData { get; private set; }

        public MainController(MainWindow mainWindow, EventData eventData) {
            this.MainWindow = mainWindow;
            this.EventData = eventData;

            this.RoundData = new(eventData);
            this.RoundDataCollection.Add(this.RoundData);

            mainWindow.AddHandler(MatchCard.MatchCardUpdateEvent, new RoutedEventHandler(OnMatchCardUpdate));
            mainWindow.OnRoundData += this.OnRoundDataHnd;
            mainWindow.OnEventData += this.OnEventDataHnd;
        }

        private void OnEventDataHnd(object? sender, EventData e) {
            Debug.WriteLine($"EventData Event: lanes {e.LaneCount}, format {e.MatchFormat}, ends {e.DefaultEnds}");
            this.EventData = e;
            this.SyncRoundData(e);
            if (this.RoundData != default) this.PopulateMatchCards();
        }

        private void OnRoundDataHnd(object? sender, MainWindow.RoundDataEventArgs e) {
            Debug.WriteLine($"RoundData Event: {e.Action} for index {e.Index}");

            switch (e.Action){
                case "Create":
                    RoundData roundData = new(this.EventData);
                    this.RoundDataCollection.Insert(e.Index, roundData);
                    break;
                case "Remove":
                    this.RoundDataCollection.RemoveAt(e.Index);
                    break;
                case "Select":
                    this.RoundData = this.RoundDataCollection[e.Index];
                    this.PopulateMatchCards();
                    break;
                case "Copy":
                    this.RoundDataCollection.Add(this.RoundData.Copy());
                    break;
                case "Show":
                    TableViewer tv = new TableViewer();                    
                    foreach (RoundData round in this.RoundDataCollection) {
                        tv.Append($"Round {this.RoundDataCollection.IndexOf(round) + 1}:");
                        tv.Append(round);
                    }
                    tv.Show();
                    break;
                default:
                    throw new NotSupportedException($"Action '{e.Action}' is not supported.");
            }
        }

        private void OnMatchCardUpdate(object sender, RoutedEventArgs e) {
            if (e is not MatchCardEventArgs args) return;
            Debug.WriteLine($"MatchCard Update: {args.Field} for lane {args.Lane}");

            switch (args.Field) {
                case "Name":
                    if (e is not MatchCardNewArgs nameArgs) return;
                    this.UpdateName(nameArgs);
                    break;
                case "Ends":
                    if (e is not MatchCardNewArgs endsArgs) return;
                    this.RoundData[endsArgs.Lane].Ends = (int)endsArgs.NewValue;
                    break;
                case "Tie":
                    if (e is not MatchCardNewArgs tieArgs) return;
                    this.RoundData[tieArgs.Lane].TieBreaker = (int)tieArgs.NewValue;
                    break;
                case "Bowls":
                    if (e is not MatchCardNewArgs bowlsArgs) return;
                    if (bowlsArgs.Team is null) throw new ArgumentException("Team index must be provided for bowls changes.");
                    this.RoundData[bowlsArgs.Lane].Score[(int)bowlsArgs.Team] = (int)bowlsArgs.NewValue;
                    break;
                case "Format":
                    if (e is not MatchCardNewArgs formatArgs) return;
                    this.RoundData[args.Lane].MatchFormat = (MatchFormat)formatArgs.NewValue;
                    this.PopulateMatchCards();
                    break;
                case "Remove":
                    this.RoundData.RemoveAt(args.Lane);
                    this.PopulateMatchCards();
                    break;
            }
        }

        private void UpdateName(MatchCardNewArgs args) {
            if (args.Team is null) throw new ArgumentException("Team index must be provided for player name changes.");
            if (args.Position is null) throw new ArgumentException("Position must be provided for player name changes.");

            var name = (string)args.NewValue;
            var lane = args.Lane;
            var team = (int)args.Team;
            var pos = (int)args.Position;

            if (this.RoundData.PollPlayer(name) == (lane, team, pos)) {
                return; // No change needed, player is already in the correct position
            }

            // If the player already exists in the round data, remove them from their current position
            // and remove their name from the previous match card.
            if (this.RoundData.HasPlayer(name)) {
                var existing = this.RoundData.PollPlayer(name);
                this.MainWindow.GetMatchCard(existing.Item1).GetTeamCard(existing.Item2)!.SetName("", existing.Item3);
                this.RoundData.RemovePlayer(name);
            }

            this.RoundData.SetPlayer((string)args.NewValue, args.Lane, (int)args.Team, (int)args.Position);
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
            // Update data by removing empty lanes until the number of lanes matches the event data's lane count.
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

        /// <summary>
        /// Fill matchRow cards with values from "roundRow".
        /// Clears all matchRow cards that does not have a value in "roundRow".
        /// </summary>
        /// <param name="roundRow"></param>
        public void PopulateMatchCards() {
            this.MainWindow.MatchCardStackPanel.Children.Clear();

            foreach (MatchData matchData in this.RoundData) {
                MatchCard matchCard = MatchCardFactory.GenerateMatchCard(matchData.MatchFormat);
                matchCard.Lane = matchData.Lane;
                matchCard.Ends = matchData.Ends;
                this.MainWindow.MatchCardStackPanel.Children.Add(matchCard);

                matchCard.Loaded += (s, e) => {
                    for (int team = 0; team < matchData.Players.Length; team++) {
                        for (int position = 0; position < matchData.Players[team].Length; position++) {
                            TeamCard teamCard = matchCard.GetTeamCard(team)!;
                            teamCard.SetName(this.RoundData[matchData.Lane].Players[team][position], position);
                        }
                    }

                    for (int i = 0; i < matchData.Score.Length; i++) {
                        TeamCard teamCard = matchCard.GetTeamCard(i)!;
                        teamCard.Bowls = matchData.Score[i];
                    }

                    if (matchData.TieBreaker >= 0) {
                        matchCard.SetTieBreaker(matchData.TieBreaker);
                    }
                };

                foreach (TeamCard teamCard in matchCard.Descendants<TeamCard>()) {
                    teamCard.TeamIndex = teamCard.TeamIndex;
                    teamCard.Bowls = matchData.Score[teamCard.TeamIndex];
                }
            }
        }

    }
}
