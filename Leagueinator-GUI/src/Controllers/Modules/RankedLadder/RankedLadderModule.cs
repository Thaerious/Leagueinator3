using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Diagnostics;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.RankedLadder {
    public class RankedLadderModule : BaseModule {

        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.GenerateRound);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Event Results"], this.ViewEventResults);
            NamedEvent.RegisterHandler(this);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Generate Next Round"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Event Results"]);
            NamedEvent.RemoveHandler(this);
        }

        internal void ViewEventResults(object sender, RoutedEventArgs args) {
            PrintWindow pw = new(this.MainController.EventData);
            pw.Show();
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
