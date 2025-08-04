using Leagueinator.GUI.Controllers.Algorithms;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.ViewModel;
using Leagueinator.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    public class MotleyModule : BaseModule {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["View", "ELO"], this.EloMenuClick);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Export CSV"], this.ExportCSV);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Assign Players"], this.SeedPlayers);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.GenerateRound);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "ELO"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Export CSV"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Assign Players"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Generate Next Round"]);
        }

        private void ExportCSV(object sender, RoutedEventArgs e) {
            string sb = "";
            sb += $"date,name,round,lane,team,score,tb,ends,name\n";
            var recrods = this.MainController.LeagueData
                              .SelectMany(eventData => eventData.Records());

            foreach (PlayerRecord record in this.MainController.LeagueData.Records()) sb += $"{record.ToCSV()}";

            TextViewer tv = new();
            tv.Append(sb);
            tv.Show();
        }

        private void SeedPlayers(object sender, RoutedEventArgs e) {
            RoundData matchesAssigned = AssignPlayers.Assign(this.MainController.LeagueData, this.MainController.EventData, this.MainController.RoundData);
            matchesAssigned.Fill(this.MainController.EventData);

            try {
                var lanesAssigned = new AssignLanes(this.MainController.EventData, matchesAssigned).Run();
                this.MainController.AddRound(lanesAssigned);
            }
            catch (AlgoLogicException ex) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["message"] = ex.Message
                });

                this.MainController.DoDeleteRound();
                this.MainController.AddRound(matchesAssigned);
            }
        }

        private void GenerateRound(object sender, RoutedEventArgs e) {
            RoundData matchesAssigned = AssignPlayers.Assign(this.MainController.LeagueData, this.MainController.EventData, this.MainController.RoundData);
            matchesAssigned.Fill(this.MainController.EventData);

            try {
                var lanesAssigned = new AssignLanes(this.MainController.EventData, matchesAssigned).Run();
                this.MainController.AddRound(lanesAssigned);
            }
            catch (AlgoLogicException ex) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["message"] = ex.Message
                });
                this.MainController.AddRound(matchesAssigned);
            }
            
        }

        private void EloMenuClick(object sender, RoutedEventArgs e) {
            Dictionary<string, int> elo = ELO.CalculateELO(this.MainController.LeagueData);
            var ordered = elo.OrderBy(kvp => kvp.Value);

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
