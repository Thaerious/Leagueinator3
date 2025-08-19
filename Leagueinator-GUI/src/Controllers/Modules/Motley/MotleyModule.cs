using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Leagueinator.Utility.Extensions;
using System.Windows;
using Utility;

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
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.GenerateRound);
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

        private List<(string Name, List<EventResult> List, EventResult Sum)> EventScores(EventData eventData) {
            DefaultDictionary<string, List<EventResult>> dictionary = new((_) => []);

            foreach (string name in eventData.AllNames()) {
                dictionary[name].Add(new(eventData, name));
            }

            return dictionary.Select(kvp => (Name: kvp.Key, List: kvp.Value, Sum: kvp.Value.Sum()))
                             .OrderBy(tuple => tuple.Sum)
                             .ToList();
        }


        private List<(string Name, int Score, int SF, int SA)> CalculateScores(EventData eventData) {
            DefaultDictionary<string, int> dictionary = new(0);
            DefaultDictionary<string, int> sf = new(0);
            DefaultDictionary<string, int> sa = new(0);

            foreach (string name in eventData.AllNames()) {
                var teams = eventData.AllTeams().Where(t => t.Names.Contains(name));
                foreach (TeamData team in teams) {
                    dictionary[name] += ResultValue[team.Result];
                    sf[name] += team.Shots;
                    sa[name] += team.ShotsAgainst;
                }
            }

            return dictionary.Select(kvp => (Name: kvp.Key, Score: kvp.Value, SF: sf[kvp.Key], SA: sa[kvp.Key]))
                             .OrderBy(tuple => tuple.Score)
                             .ThenBy(tuple => sf[tuple.Name] - sa[tuple.Name])
                             .ThenBy(tuple => (double)sf[tuple.Name] / ((double)sf[tuple.Name] + (double)sa[tuple.Name]))
                             .Reverse()
                             .ToList();
        }

        private void ShowEventResults(object sender, RoutedEventArgs e) {
            var scores = this.CalculateScores(this.MainController.EventData);
            var resultsWindow = new ResultsWindow();

            int pos = 1;
            foreach ((string Name, int Score, int SF, int SA) score in scores) {
                resultsWindow.AddHeader(
                    [$"#{pos++} {score.Name} ({score.Score})", "Result", "SF", "SA"],
                    [150, 70, 40, 40]
                );

                var teams = this.MainController.EventData.AllTeams().Where(t => t.Names.Contains(score.Name));
                foreach (TeamData team in teams) {
                    resultsWindow.AddRow(
                        [$"Lane {team.Parent.Lane + 1}", $"{team.Result}", $"{team.Shots}", $"{team.ShotsAgainst}"]
                    );
                }

                double pct = (double)score.SF / ((double)score.SF + (double)score.SA) * 100.0;
                resultsWindow.FinishTable(
                    $"sf:{score.SF},  sa:{score.SA}, diff:{score.SF - score.SA},  pct:{pct:0.00}"
                );
            }

            resultsWindow.Show();
        }

        private void ShowLeagueResults(object sender, RoutedEventArgs e) {
            var scores = this.LeagueScores(this.MainController.LeagueData);
            var resultsWindow = new ResultsWindow();
            var eventResults = this.LeagueScores(this.MainController.LeagueData);

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



        private void GenerateRound(object sender, RoutedEventArgs e) {
            //SRCRoundData matchesAssigned = AssignPlayers.Assign(this.MainController.LeagueData, this.MainController.EventData, this.MainController.SRCRoundData);
            //matchesAssigned.Fill(this.MainController.EventData);

            //try {
            //    var lanesAssigned = new AssignLanes(this.MainController.EventData, matchesAssigned).Run();
            //    this.MainController.AddRound(lanesAssigned);
            //}
            //catch (AlgoLogicException ex) {
            //    this.DispatchEvent(EventName.Notification, new() {
            //        ["message"] = ex.Message
            //    });
            //    this.MainController.AddRound(matchesAssigned);
            //}            
        }
    }
}
