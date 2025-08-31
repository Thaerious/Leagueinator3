using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Forms.Results;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Leagueinator.Utility.Extensions;
using System.Windows;
using Utility.Collections;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.ScoringPlus {

    public class ScoringPlusModule : BaseModule {

        public static readonly Dictionary<GameResult, int> ResultValue = new() {
            { GameResult.Vacant, 0 },
            { GameResult.Loss, 1 },
            { GameResult.Tie, 2 },
            { GameResult.Win, 3 }
        };

        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "Event by Team"], this.ShowEventResults);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "League by Team"], this.ShowLeagueResults);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "Event by Player"], this.ShowEventResults);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Results", "League by Player"], this.ShowLeagueResults);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "Event by Team"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "Event by Player"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "League by Team"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Results", "League by Player"]);
            NamedEvent.RemoveHandler(this);
        }       

        private void ShowEventResults(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }


        private void ShowLeagueResults(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        public List<string> EventRankingByPlayer(LeagueData leagueData) {
            throw new NotImplementedException();
        }

        public List<string> LeagueRankingByPlayer(LeagueData leagueData) {
            throw new NotImplementedException();
        }
    }
}
