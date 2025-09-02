using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Forms.Results;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Numerics;
using System.Windows;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules {

    public class ScoringModule<T> : BaseModule where T : IResult<T> {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "Event by Team"], (s, e) => ShowEventResultsByTeam(this.MainController.EventData));
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "League by Team"], (s, e) => ShowLeagueResultsByTeam(this.MainController.LeagueData));
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "Event by Player"], (s, e) => ShowEventResultsByPlayer(this.MainController.EventData));
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "League by Player"], (s, e) => ShowLeagueResultsByPlayer(this.MainController.LeagueData));
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "Event by Team"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "Event by Player"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "League by Team"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "League by Player"]);
            NamedEvent.RemoveHandler(this);
        }

        private static void ShowLeagueResultsByPlayer(LeagueData leagueData) {
            var form = new ResultsForm("League Results by Player", leagueData);

            form.CollectionChanged += (s, eventDataSet) => {
                var results = new ResultsCollection<T>();
                foreach (EventData eventData in eventDataSet) results.AddTeams(eventData);
                form.Clear();

                foreach (string player in results.Players) {
                    form.AddHeader([$"#{results.Rank(player)}", player, .. T.Labels],
                                   [40, 150, .. T.ColSizes]
                                  );

                    results[player].ForEach(
                        result => form.AddRow(["", $"Lane {result.Lane + 1}", .. result.Cells()])
                    );

                    var summaryCells = results[player].Sum().Cells();
                    summaryCells[0] = results[player].Sum().Score.ToString();
                    form.AddSummaryRow(["", "", .. summaryCells]);
                    form.FinishTable($"diff:{results[player].Sum().Diff} pct:{results[player].Sum().PCT:F3}");
                }
            };

            form.Show();
        }

        private static void ShowLeagueResultsByTeam(LeagueData leagueData) {
            var form = new ResultsForm("League Results by Team", leagueData);

            form.CollectionChanged += (s, eventDataSet) => {
                var results = new ResultsCollection<T>();
                foreach (EventData eventData in eventDataSet) results.AddTeams(eventData);
                form.Clear();

                foreach (Players team in results.Teams) {
                    form.AddHeader([$"#{results.Rank(team)}", team.JoinString(), .. T.Labels],
                                   [40, 150, .. T.ColSizes]
                                  );

                    results[team].ForEach(
                        result => form.AddRow(["", $"Lane {result.Lane + 1}", .. result.Cells()])
                    );

                    var summaryCells = results[team].Sum().Cells();
                    summaryCells[0] = results[team].Sum().Score.ToString();
                    form.AddSummaryRow(["", "", .. summaryCells]);
                    form.FinishTable($"diff:{results[team].Sum().Diff} pct:{results[team].Sum().PCT:F3}");
                }
            };

            form.Show();
        }

        private static void ShowEventResultsByTeam(EventData eventData) {
            var form = new ResultsWindow("Event Results by Team");
            var results = new ResultsCollection<T>(eventData);

            foreach (Players team in results.Teams) {
                form.AddHeader([$"#{results.Rank(team)}", team.JoinString(), .. T.Labels], 
                               [40, 150, .. T.ColSizes]
                              );

                results[team].ForEach(
                    result => form.AddRow(["", $"Lane {result.Lane + 1}", .. result.Cells()])
                );

                var summaryCells = results[team].Sum().Cells();
                summaryCells[0] = results[team].Sum().Score.ToString();
                form.AddSummaryRow(["", "", ..summaryCells]);
                form.FinishTable($"");
            }

            form.Show();
        }

        private static void ShowEventResultsByPlayer(EventData eventData) {
            var form = new ResultsWindow("Event Results by Team");
            var results = new ResultsCollection<T>(eventData);

            foreach (string player in results.Players) {
                form.AddHeader([$"#{results.Rank(player)}", player, .. T.Labels],
                               [40, 150, .. T.ColSizes]
                              );

                results[player].ForEach(
                    result => form.AddRow(["", $"Lane {result.Lane + 1}", .. result.Cells()])
                );

                var summaryCells = results[player].Sum().Cells();
                summaryCells[0] = results[player].Sum().Score.ToString();
                form.AddSummaryRow(["", "", .. summaryCells]);
                form.FinishTable($"");
            }

            form.Show();
        }
    }
}
