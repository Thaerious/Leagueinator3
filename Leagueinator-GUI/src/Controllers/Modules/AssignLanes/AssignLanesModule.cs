using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.AssignLanes {
    public class AssignLanesModule : BaseModule {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Assign Lanes"], this.AssignLanes);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Assign Lanes"]);
        }

        private void AssignLanes(object sender, RoutedEventArgs e) {
            this.DispatchEvent(EventName.AssignLanes, []);
        }

        [NamedEventHandler(EventName.AssignLanes)]
        internal void DoAssignLanes() {
            var assignLanes = new AssignLanes(this.MainController.EventData, this.MainController.RoundData);
            var newRound = assignLanes.Run();
            this.MainController.EventData.ReplaceRound(this.MainController.RoundData, newRound);
            this.MainController.DispatchModel();
            this.MainController.DispatchSetTitle(false);
        }
    }
}
