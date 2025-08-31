using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Forms.Results;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Leagueinator.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.ScoringBowls {

    public class ScoringBowlsModule : BaseModule {

        public static readonly Dictionary<GameResult, int> ResultValue = new() {
            { GameResult.Vacant, 0 },
            { GameResult.Loss, 1 },
            { GameResult.Tie, 2 },
            { GameResult.Win, 3 }
        };

        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "Event by Team"], (s, e)=> ShowResultsByTeam<BowlsResult>(this.MainController.EventData));
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "League by Team"], (s, e) => ShowLeagueResultsByTeam<BowlsResult>(this.MainController.LeagueData));
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "Event by Player"], (s, e) => ShowResultsByPlayer<BowlsResult>(this.MainController.EventData));
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "League by Player"], (s, e) => ShowLeagueResultsByPlayer<BowlsResult>(this.MainController.LeagueData));
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "Event by Team"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "Event by Player"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "League by Team"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "League by Player"]);
            NamedEvent.RemoveHandler(this);
        }

        private void ShowLeagueResultsByPlayer<T>(LeagueData leagueData) {
            var form = new ResultsForm("League Results", leagueData);
            form.CollectionChanged += (s, eventDataSet) => {
                var results = new ResultsCollection<BowlsResult>();
                foreach(EventData eventData in eventDataSet) results.AddTeams(eventData);
                form.Clear();

                form.AddHeader(BowlsResult.Labels, BowlsResult.ColSizes);
                foreach (string player in results.Players) {
                    results[player].ForEach(
                        result => form.AddRow(result.Cells())
                    );                    
                }
            };
        }

        private void ShowLeagueResultsByTeam<T>(LeagueData leagueData) {
            var form = new ResultsForm("League Results", leagueData);
        }

        private static void ShowResultsByTeam<T>(IHasTeams hasTeams) where T : IResult<T> {
            var results = new ResultsCollection<T>(hasTeams);
            var resultsWindow = new ResultsWindow("Event Results");


            foreach (Players players in results.Teams) {
                T sum = results[players].Sum();
                
                //resultsWindow.AddHeader(
                    //[$"#{results.Rank(players)} {players}", .. BowlsResult.Labels],
                    //T.Labels
                //);

                foreach (T result in results[players]) {
                    resultsWindow.AddRow(result.Cells());
                }

                resultsWindow.AddSummaryRow(sum.Cells());

                // TODO Re-enable
                //resultsWindow.FinishTable(
                //    $"diff:{score.Sum.Diff},  pct:{score.Sum.PCT * 100:0.00}"
                //);
            }

            resultsWindow.Show();
        }

        private static void ShowResultsByPlayer<T>(IHasTeams hasTeams) where T : IResult<T> {
            var results = new ResultsCollection<T>(hasTeams);
            var resultsWindow = new ResultsWindow("Event Results");

            foreach (string player in results.Players) {
                T sum = results[player].Sum();

                resultsWindow.AddHeader(
                    [$"#{results.Rank(player)} {player}", .. BowlsResult.Labels],
                    [100, 100, 40, 40, 40, 40, 100]
                );

                foreach (T result in results[player]) {
                    resultsWindow.AddRow(result.Cells());
                }

                resultsWindow.AddSummaryRow(sum.Cells());

                // TODO Re-enable
                //resultsWindow.FinishTable(
                //    $"diff:{score.Sum.Diff},  pct:{score.Sum.PCT * 100:0.00}"
                //);
            }

            resultsWindow.Show();
        }

    }
}
