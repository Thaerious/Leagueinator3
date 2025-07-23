using Leagueinator.GUI.Controllers.Algorithms;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.RankedLadder {
    public class RankedLadderModule : BaseModule {

        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.MenuGenerate);
            //this.MainWindow.MainMenu.AddMenuItem(["View", "Results By Team"], this.ViewResults);
        }

        public override void UnloadModule() {
        }

        private void MenuGenerate(object sender, RoutedEventArgs args) {
            this.MainWindow.ClearFocus();
            this.DispatchEvent(EventName.GenerateRound);
        }

        [NamedEventHandler(EventName.GenerateRound)]
        internal void DoGenerateRound() {
            RankedLadderRoundBuilder builder = new(this.MainController.EventData);
            RoundData newRound = builder.GenerateRound();
            AssignLanes assignLanes = new(this.MainController.EventData, newRound);
            newRound = assignLanes.DoAssignment();
            this.MainController.AddRound(newRound);
        }
    }
}
