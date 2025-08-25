using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Diagnostics;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.ELO {
    public class ELOModule : BaseModule {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["View", "ELO - Event"], this.EventEloMenuClick);
            this.MainWindow.MainMenu.AddMenuItem(["View", "ELO - League"], this.LeagueEloMenuClick);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Seed Players"], this.SeedPlayers);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "ELO - Event"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "ELO - League"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Seed Players"]);
        }

        /// <summary>
        /// Take all the players from the target round and assign them partners.
        /// Returns a round with the players in teams; the teams in no particular order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeedPlayers(object sender, RoutedEventArgs e) {
            BinSeedPlayers assignPlayers = new(this.MainController.RoundData);
            RoundData playersAssigned = assignPlayers.NewRound();
            Debug.WriteLine($"Final Round Fitness {playersAssigned.Fitness()}");
            playersAssigned.Fill();
            this.MainController.EventData.ReplaceRound(this.MainController.RoundData, playersAssigned);
            this.MainController.DispatchModel();
        }

        // Display ELODictionary to user,
        private void EventEloMenuClick(object sender, RoutedEventArgs e) {
            ELODictionary elo = new(this.MainController.LeagueData);
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

        private void LeagueEloMenuClick(object sender, RoutedEventArgs e) {
            ELODictionary elo = new(this.MainController.LeagueData);
            var names = this.MainController.LeagueData.AllNames();
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
