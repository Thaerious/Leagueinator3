using Algorithms;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.AssignLanes {
    public class AssignLanesModule : BaseModule {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Assign Lanes"], this.HndAssignLanes);
            NamedEvent.RegisterHandler(this);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Assign Lanes"]);
            NamedEvent.RemoveHandler(this);
        }

        private void HndAssignLanes(object sender, RoutedEventArgs e) {
            this.DispatchEvent(EventName.AssignLanes, []);
        }

        [NamedEventHandler(EventName.AssignLanes)]
        internal void DoAssignLanes() {
            try {
                var newRound = LaneAssigner.NewRound(this.MainController.RoundData);
                this.MainController.EventData.ReplaceRound(this.MainController.RoundData, newRound);
                this.MainController.DispatchModel();
                this.MainController.DispatchSetTitle(false);
            }
            catch (UnsolvedException ex) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["alertLevel"] = AlertLevel.Inform,
                    ["message"] = "Could not assign unique lanes to all teams."
                });
            }
        }
    }
}
