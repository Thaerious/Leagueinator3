using Algorithms;
using Leagueinator.GUI.Controllers.Modules.AssignLanes;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Windows;
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

        private void ShowEventResults(object sender, RoutedEventArgs e) {
            var scores = RoundResult.EventScores(this.MainController.EventData);
            var resultsWindow = new ResultsWindow("Event Results");

            int pos = 1;
            foreach ((TeamData Team, List<RoundResult> List, RoundResult Sum) score in scores) {
                resultsWindow.AddHeader(
                    [$"#{pos++} {score.Team.Names.JoinString()} ({score.Sum.Score})", "R", "SF", "SA", "TB", "Ends", "VS"],
                    [150, 40, 40, 40, 40, 40, 150]
                );

                foreach (RoundResult rr in score.List) {
                    resultsWindow.AddRow(
                        [$"Lane {rr.Lane + 1}", $"{rr.Result.ToString()[0]}", $"{rr.BowlsFor}+{rr.PlusFor}", $"{rr.BowlsAgainst}+{rr.PlusAgainst}", $"{rr.TieBreaker.ToString()[0]}", $"{rr.Ends}", $"{rr.Opponents.JoinString()}"]
                    );
                }

                resultsWindow.AddSummaryRow(
                    [$"", $"{score.Sum.Score}", $"{score.Sum.BowlsFor}+{score.Sum.PlusFor}", $"{score.Sum.BowlsAgainst}+{score.Sum.PlusAgainst}", "", $"{score.Sum.Ends}", ""]
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
            RoundData? newRound = default;

            try {
                this.MainWindow.ClearFocus();
                newRound = RoundFactory.Generate(this.MainController.EventData);
                newRound = LaneAssigner.NewRound(newRound);
                this.MainController.AddRound(newRound);
            }
            catch (UnsolvedException ex) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["alertLevel"] = AlertLevel.Inform,
                    ["message"] = "Could not assign unique lanes to all teams."
                });
            }
            finally {
                if (newRound != default) this.MainController.AddRound(newRound);
            }
        }
    }
}
