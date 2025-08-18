using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.ELO {
    public class ELOModule : BaseModule {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["View", "ELO"], this.EloMenuClick);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Seed Players"], this.SeedPlayers);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "ELO"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Seed Players"]);
        }

        /// <summary>
        /// Take all the players from the target round and assign them partners.
        /// Returns a round with the players in teams; the teams in no particular order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeedPlayers(object sender, RoutedEventArgs e) {
            SeedPlayers assignPlayers = new(this.MainController.LeagueData, this.MainController.EventData, this.MainController.RoundData);
            RoundData playersAssigned = assignPlayers.Run();
            playersAssigned.Fill();
            this.MainController.EventData.ReplaceRound(this.MainController.RoundData, playersAssigned);
            this.MainController.DispatchModel();
        }

        // Display ELOEngine to user,
        private void EloMenuClick(object sender, RoutedEventArgs e) {
            ELOEngine elo = new(this.MainController.LeagueData);
            var names = this.MainController.EventData.AllNames();
            var ordered = elo.Where(kvp => names.Contains(kvp.Key))                        
                             .OrderBy(kvp => kvp.Value)
                             .Reverse();

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
