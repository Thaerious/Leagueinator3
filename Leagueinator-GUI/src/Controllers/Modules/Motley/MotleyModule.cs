using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    public class MotleyModule : BaseModule {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["View", "ELO"], this.EloMenuClick);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Seed Players"], this.SeedPlayers);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.GenerateRound);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "ELO"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Seed Players"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Generate Next Round"]);
        }

        /// <summary>
        /// Take all the players from the target round and assign them partners.
        /// Returns a round with the players in teams; the teams in no particular order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeedPlayers(object sender, RoutedEventArgs e) {
            AssignPlayers assignPlayers = new(this.MainController.LeagueData, this.MainController.EventData, this.MainController.RoundData);
            RoundData playersAssigned = assignPlayers.Run();
            playersAssigned.Fill();
            this.MainController.EventData.ReplaceRound(this.MainController.RoundData, playersAssigned);
            this.MainController.DispatchModel();

            //matchesAssigned.Fill(this.MainController.EventData);

            //try {
            //    var lanesAssigned = new AssignLanes(this.MainController.EventData, matchesAssigned).Run();
            //    this.MainController.AddRound(lanesAssigned);
            //}
            //catch (AlgoLogicException ex) {
            //    this.DispatchEvent(EventName.Notification, new() {
            //        ["message"] = ex.Message
            //    });

            //    this.MainController.DoDeleteRound();
            //    this.MainController.AddRound(matchesAssigned);
            //}
        }

        private void GenerateRound(object sender, RoutedEventArgs e) {
            //RoundData matchesAssigned = AssignPlayers.Assign(this.MainController.LeagueData, this.MainController.EventData, this.MainController.RoundData);
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

        private void EloMenuClick(object sender, RoutedEventArgs e) {
            Dictionary<string, int> elo = ELO.CalculateELO(this.MainController.LeagueData);
            var ordered = elo.OrderBy(kvp => kvp.Value).Reverse();

            string sb = "";
            foreach (var kvp in ordered) {
                sb += $"{kvp.Key} : {kvp.Value}\n";
            }

            this.DispatchEvent(EventName.DisplayText, new() {
                ["text"] = sb
            });
        }
    }
}
