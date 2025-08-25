using Leagueinator.GUI.Controllers.Modules.AssignLanes;
using Leagueinator.GUI.Controllers.Modules.ELO;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Leagueinator.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using Utility;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.Motley {

    public class MotleyModule : BaseModule {

        public static readonly Dictionary<GameResult, int> ResultValue = new() {
            { GameResult.Vacant, 0 },
            { GameResult.Loss, 1 },
            { GameResult.Draw, 2 },
            { GameResult.Win, 3 }
        };

        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.NewRound);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Event Results"], this.ShowEventResults);
            this.MainWindow.MainMenu.AddMenuItem(["View", "League Results"], this.ShowLeagueResults);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Generate Next Round"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Event Results"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "League Results"]);
            NamedEvent.RemoveHandler(this);
        }

        private List<(string Name, List<EventResult> List, EventResult Sum)> LeagueScores(LeagueData leagueData) {
            DefaultDictionary<string, List<EventResult>> dictionary = new((_) => []);

            foreach (EventData eventData in leagueData.Events) {
                foreach (string name in eventData.AllNames()) {
                    dictionary[name].Add(new(eventData, name));
                }
            }

            return dictionary.Select(kvp => (Name: kvp.Key, List: kvp.Value, Sum: kvp.Value.Sum()))
                             .OrderBy(tuple => tuple.Sum)
                             .ToList();
        }

        private List<(string Name, List<RoundResult> List, RoundResult Sum)> EventScores(EventData eventData) {
            DefaultDictionary<string, List<RoundResult>> dictionary = new((_) => []);

            foreach (TeamData teamData in eventData.AllTeams()) {
                if (teamData.IsEmpty()) continue;
                foreach (string name in teamData.Names) {
                    if (!string.IsNullOrEmpty(name)) {
                        dictionary[name].Add(new(name, teamData));
                    }
                }
            }

            return dictionary.Select(kvp => (Name: kvp.Key, List: kvp.Value, Sum: kvp.Value.Sum()))
                             .OrderBy(tuple => tuple.Sum)
                             .ToList();
        }

        private void ShowEventResults(object sender, RoutedEventArgs e) {
            var scores = this.EventScores(this.MainController.EventData);
            var resultsWindow = new ResultsWindow("Event Results");

            int pos = 1;
            foreach ((string Name, List<RoundResult> List, RoundResult Sum) score in scores) {
                resultsWindow.AddHeader(
                    [$"#{pos++} {score.Name} ({score.Sum.Score})", "Partners", "PTS", "SF", "SA", "TB", "Ends", "Opponents"],
                    [100, 100, 40, 40, 40, 40, 40, 100]
                );

                foreach (RoundResult rr in score.List) {
                    resultsWindow.AddRow(
                        [$"Lane {rr.Lane + 1}", $"{rr.Partners}",  $"{rr.Score}", $"{rr.ShotsFor}", $"{rr.ShotsAgainst}", $"{rr.TieBreaker.ToString()[0]}", $"{rr.Ends}", $"{rr.Opponents}"]
                    );
                }

                resultsWindow.AddSummaryRow(
                    [$"", "", $"{score.Sum.Score}", $"{score.Sum.ShotsFor}", $"{score.Sum.ShotsAgainst}", "", $"{score.Sum.Ends}", ""]
                );

                resultsWindow.FinishTable(
                    $"diff:{score.Sum.Diff},  pct:{score.Sum.PCT * 100:0.00}"
                );
            }

            resultsWindow.Show();
        }


        private void ShowLeagueResults(object sender, RoutedEventArgs e) {
            var scores = this.LeagueScores(this.MainController.LeagueData);
            var resultsWindow = new ResultsWindow("League Results");

            int pos = 1;
            foreach ((string Name, List<EventResult> List, EventResult Sum) score in scores) {
                resultsWindow.AddHeader(
                    [$"#{pos++} {score.Name} ({score.Sum.Score})", "GP", "PTS", "W", "T", "L", "SF", "SA"],
                    [150, 40, 40, 40, 40, 40, 40, 40]
                );

                foreach (EventResult er in score.List) {
                    resultsWindow.AddRow(
                        [$"{er.Label}", $"{er.GamesPlayed}", $"{er.Score}", $"{er.Wins}", $"{er.Draws}", $"{er.Losses}", $"{er.ShotsFor}", $"{er.ShotsAgainst}"]
                    );
                }

                resultsWindow.AddSummaryRow(
                        [$"", $"{score.Sum.GamesPlayed}", $"{score.Sum.Score}", $"{score.Sum.Wins}", $"{score.Sum.Draws}", $"{score.Sum.Losses}", $"{score.Sum.ShotsFor}", $"{score.Sum.ShotsAgainst}"]
                    );

                resultsWindow.FinishTable(
                    $"diff:{score.Sum.Diff},  pct:{score.Sum.PCT * 100:0.00}"
                );
            }

            resultsWindow.Show();
        }



        private void NewRound(object sender, RoutedEventArgs e) {
            RoundData current = this.MainController.RoundData;
            BinSeedPlayers assignPlayers = new(current);
            RoundData playersAssigned = assignPlayers.NewRound();
            playersAssigned.Fill();
            RoundData lanesAssigned = LaneAssigner.NewRound(playersAssigned);
            this.MainController.EventData.AddRound(lanesAssigned);
            this.MainController.DispatchModel();
        }
    }
}
