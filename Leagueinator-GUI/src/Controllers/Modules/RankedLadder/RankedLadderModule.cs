using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using Utility;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.RankedLadder {
    public class RankedLadderModule : BaseModule {

        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.GenerateRound);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Event Results"], this.ShowEventResults);
            NamedEvent.RegisterHandler(this);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Generate Next Round"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Event Results"]);
            NamedEvent.RemoveHandler(this);
        }

        private List<(Players Names, List<RoundResult> List, RoundResult Sum)> EventScores(EventData eventData) {
            DefaultDictionary<Players, List<RoundResult>> dictionary = new((_) => []);

            foreach (TeamData teamData in eventData.AllTeams()) {
                if (teamData.IsEmpty()) continue;
                RoundResult rr = new(teamData);                
                rr.Opponents = [.. teamData.GetOpposition().AllNames()];
                dictionary[new(teamData.Names)].Add(rr);
            }

            return dictionary.Select(kvp => (Names: kvp.Key, List: kvp.Value, Sum: kvp.Value.Sum()))
                             .OrderBy(tuple => tuple.Sum)
                             .ToList();
        }

        private void ShowEventResults(object sender, RoutedEventArgs e) {
            var scores = this.EventScores(this.MainController.EventData);
            var resultsWindow = new ResultsWindow();

            int pos = 1;
            foreach ((Players Names, List<RoundResult> List, RoundResult Sum) score in scores) {
                resultsWindow.AddHeader(
                    [$"#{pos++} {score.Names} ({score.Sum.Score})", "R", "SF", "SA", "Ends", "VS"],
                    [150, 40, 40, 40, 40, 150]
                );

                foreach (RoundResult rr in score.List) {
                    resultsWindow.AddRow(
                        [$"Lane {rr.Lane + 1}", $"{rr.Result.ToString()[0]}", $"{rr.BowlsFor}+{rr.PlusFor}", $"{rr.BowlsAgainst}+{rr.PlusAgainst}", $"{rr.Ends}", $"{rr.Opponents.JoinString()}"]
                    );
                }

                resultsWindow.AddSummaryRow(
                    [$"", $"{score.Sum.Score}", $"{score.Sum.BowlsFor}+{score.Sum.PlusFor}", $"{score.Sum.BowlsAgainst}+{score.Sum.PlusAgainst}", $"{score.Sum.Ends}", ""]
                );

                resultsWindow.FinishTable(
                    $"diff:{score.Sum.DiffBowls}+{score.Sum.DiffPlus},  pct:{score.Sum.PCT * 100:0.00}"
                );
            }

            resultsWindow.Show();
        }

        private void GenerateRound(object sender, RoutedEventArgs args) {
            this.DispatchEvent(EventName.GenerateRound);
        }

        [NamedEventHandler(EventName.GenerateRound)]
        internal void DoGenerateRound() {
            Debug.WriteLine(" *** DoGenerateRound");
            this.MainWindow.ClearFocus();
            RankedLadderRoundBuilder builder = new(this.MainController.EventData);
            RoundData newRound = builder.GenerateRound();
            AssignLanes.AssignLanes assignLanes = new(this.MainController.EventData, newRound);
            newRound = assignLanes.Run();
            this.MainController.AddRound(newRound);
        }
    }
}
