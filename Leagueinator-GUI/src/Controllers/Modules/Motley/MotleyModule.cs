using Leagueinator.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    public class MotleyModule : BaseModule {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.GenerateRound);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Generate Next Round"]);
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
